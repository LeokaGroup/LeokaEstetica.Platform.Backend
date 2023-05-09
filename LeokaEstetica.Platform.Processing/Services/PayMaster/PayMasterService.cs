using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Extensions;
using LeokaEstetica.Platform.Processing.Factories;
using LeokaEstetica.Platform.Processing.Models.Output;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Processing.Services.PayMaster;

/// <summary>
/// Класс реализует методы сервиса работы с платежной системой PayMaster.
/// </summary>
public class PayMasterService : IPayMasterService
{
    private readonly ILogService _logService;
    private readonly IConfiguration _configuration;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPayMasterRepository _payMasterRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="configuration">Конфигурация внедренная через DI.</param>
    /// <param name="fareRuleRepository">Репозиторий правил.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="payMasterRepository">Сервис ПС PayMaster.</param>
    /// <param name="accessUserService">Сервис доступа пользователя.</param>
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступа пользователя.</param>
    public PayMasterService(ILogService logService,
        IConfiguration configuration,
        IFareRuleRepository fareRuleRepository,
        IUserRepository userRepository,
        IPayMasterRepository payMasterRepository, 
        IAccessUserService accessUserService, 
        IAccessUserNotificationsService accessUserNotificationsService)
    {
        _logService = logService;
        _configuration = configuration;
        _fareRuleRepository = fareRuleRepository;
        _userRepository = userRepository;
        _payMasterRepository = payMasterRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<CreateOrderOutput> CreateOrderAsync(CreateOrderInput createOrderInput, string account,
        string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                    "Для покупки тарифа должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);

                throw ex;
            }

            // Находим тариф, который оплачивает пользователь.
            var fareRule = await _fareRuleRepository.GetByIdAsync(createOrderInput.FareRuleId);

            if (fareRule is null)
            {
                throw new InvalidOperationException(
                    $"Ошибка получения тарифа. FareRuleId был {createOrderInput.FareRuleId}. " +
                    $"CreateOrder:{JsonConvert.SerializeObject(createOrderInput)}");
            }

            // Заполняем модель для запроса в ПС.
            CreateOrderRequestFactory.Create(createOrderInput, _configuration, fareRule);

            await _logService.LogInfoAsync(new ApplicationException("Начало создания заказа."));

            using var httpClient = new HttpClient();
            httpClient.SetPayMasterRequestAuthorizationHeader(_configuration);
            
            // Создаем платеж в ПС.
            var responseCreateOrder = await httpClient.PostAsJsonAsync(ApiConsts.CREATE_PAYMENT, createOrderInput);

            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var ex = new InvalidOperationException(
                    $"Ошибка создания платежа в ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            // Парсим результат из ПС.
            var order = await responseCreateOrder.Content.ReadFromJsonAsync<CreateOrderOutput>();

            // Если ошибка при парсинге заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                var ex = new InvalidOperationException(
                    $"Ошибка парсинга данных из ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            // Проверяем статус заказа в ПС.
            var responseCheckStatusOrder = await httpClient.GetStringAsync(string.Concat(ApiConsts.CHECK_PAYMENT_STATUS,
                order.PaymentId));

            // Если ошибка получения данных платежа.
            if (string.IsNullOrEmpty(responseCheckStatusOrder))
            {
                var ex = new InvalidOperationException(
                    "Ошибка проверки статуса платежа в ПС. " +
                    $"Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            var createOrder = JsonConvert.DeserializeObject<PaymentStatusOutput>(responseCheckStatusOrder);
            var createdOrder = CreatePaymentOrderFactory.Create(order.PaymentId, fareRule.Name,
                createOrderInput.Invoice.Description, userId, createOrderInput.Amount.Value, 1,
                PaymentCurrencyEnum.RUB.ToString(), DateTime.Parse(createOrder.Created), createOrder.OrderStatus,
                PaymentStatusEnum.Pending.GetEnumDescription());

            // Создаем заказ в БД.
            var createdOrderResult = await _payMasterRepository.CreateOrderAsync(createdOrder);

            // Приводим к нужному виду.
            var result = CreateOrderResultFactory.Create(createdOrderResult.OrderId.ToString(), order.Url);

            await _logService.LogInfoAsync(new ApplicationException("Конец создания заказа."));
            await _logService.LogInfoAsync(new ApplicationException("Создание заказа успешно."));

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogCriticalAsync(ex);
            throw;
        }
    }
}