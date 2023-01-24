using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Exceptions;
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

    public PayMasterService(ILogService logService,
        IConfiguration configuration,
        IFareRuleRepository fareRuleRepository,
        IUserRepository userRepository,
        IPayMasterRepository payMasterRepository)
    {
        _logService = logService;
        _configuration = configuration;
        _fareRuleRepository = fareRuleRepository;
        _userRepository = userRepository;
        _payMasterRepository = payMasterRepository;
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<CreateOrderOutput> CreateOrderAsync(CreateOrderInput createOrderInput, string account)
    {
        try
        {
            using var httpClient = new HttpClient();

            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Находим тариф, который оплачивает пользователь.
            var fareRule = await _fareRuleRepository.GetByIdAsync(createOrderInput.FareRuleId);

            if (fareRule is null)
            {
                throw new NullReferenceException(
                    $"Ошибка получения тарифа. FareRuleId был {createOrderInput.FareRuleId}. " +
                    $"CreateOrder:{JsonConvert.SerializeObject(createOrderInput)}");
            }

            // Заполняем модель для запроса в ПС.
            CreateOrderRequestFactory.Create(ref createOrderInput, _configuration, fareRule);

            // Устанавливаем заголовки.
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _configuration["Commerce:PayMaster:ApiToken"]);

            // Создаем платеж в ПС.
            var responseCreateOrder = await httpClient.PostAsJsonAsync(ApiConsts.CREATE_PAYMENT, createOrderInput);

            // Получаем результат из ПС.
            var order = await responseCreateOrder.Content.ReadFromJsonAsync<CreateOrderOutput>();

            // Если ошибка при получении заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                throw new ErrorCreateOrderException(JsonConvert.SerializeObject(createOrderInput));
            }

            // Проверяем статус заказа в ПС.
            var responseCheckStatusOrder =
                await httpClient.GetStringAsync(string.Concat(ApiConsts.CHECK_PAYMENT_STATUS, order.PaymentId));

            // Если ошибка получения данных платежа.
            if (string.IsNullOrEmpty(responseCheckStatusOrder))
            {
                throw new ErrorCreateOrderException(JsonConvert.SerializeObject(createOrderInput));
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

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}