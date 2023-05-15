using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
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
    private readonly IUserRepository _userRepository;
    private readonly ICommerceRepository _commerceRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;
    private readonly ICommerceRedisService _commerceRedisService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="configuration">Конфигурация внедренная через DI.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="accessUserService">Сервис доступа пользователя.</param>
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступа пользователя.</param>
    /// <param name="commerceRedisService">Сервис кэша коммерции.</param>
    public PayMasterService(ILogService logService,
        IConfiguration configuration,
        IUserRepository userRepository,
        ICommerceRepository commerceRepository, 
        IAccessUserService accessUserService, 
        IAccessUserNotificationsService accessUserNotificationsService, 
        ICommerceRedisService commerceRedisService)
    {
        _logService = logService;
        _configuration = configuration;
        _userRepository = userRepository;
        _commerceRepository = commerceRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
        _commerceRedisService = commerceRedisService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<CreateOrderOutput> CreateOrderAsync(CreateOrderInput createOrderInput, string account,
        string token, Guid publicId)
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
            
            // Получаем заказ из кэша.
            var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId, publicId);
            var orderCache = await _commerceRedisService.GetOrderCacheAsync(key);
            
            // Заполняем модель для запроса в ПС.
            createOrderInput = await CreateOrderRequestAsync(createOrderInput, orderCache.FareRuleName,
                orderCache.Price);

            await _logService.LogInfoAsync(new ApplicationException("Начало создания заказа."));
            
            var httpClient = await SetPayMasterRequestAuthorizationHeader();
            
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

            var result = await CreatePaymentOrderResultAsync(order, orderCache, createOrderInput, userId, httpClient);

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

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает модель запроса в ПС PayMaster.
    /// </summary>
    /// <param name="createOrderInput">Входная модель для запроса в ПС.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <param name="price">Цена.</param>
    /// <returns>Модель запроса в ПС PayMaster.</returns>
    private async Task<CreateOrderInput> CreateOrderRequestAsync(CreateOrderInput createOrderInput,
        string fareRuleName, decimal price)
    {
        // Задаем Id мерчанта (магазина).
        createOrderInput.CreateOrderRequest.MerchantId = new Guid(_configuration["Commerce:PayMaster:MerchantId"]);
        createOrderInput.CreateOrderRequest.TestMode = true; // TODO: Добавить управляющий ключ в таблицу конфигов.
        createOrderInput.CreateOrderRequest.Invoice = new Invoice
        {
            Description = "Оплата тарифа: " + fareRuleName
        };
        createOrderInput.CreateOrderRequest.Amount = new Amount
        {
            Value = price,
            Currency = PaymentCurrencyEnum.RUB.ToString()
        };
        createOrderInput.CreateOrderRequest.PaymentMethod = "BankCard";

        return await Task.FromResult(createOrderInput);
    }

    /// <summary>
    /// Метод 
    /// </summary>
    /// <param name="paymentId"></param>
    /// <param name="orderCache"></param>
    /// <param name="createOrderRequest"></param>
    /// <param name="userId"></param>
    /// <param name="responseCheckStatusOrder"></param>
    /// <returns></returns>
    private async Task<CreatePaymentOrderInput> CreatePaymentOrderRequestAsync(string paymentId,
        CreateOrderCache orderCache, CreateOrderRequest createOrderRequest, long userId,
        string responseCheckStatusOrder)
    {
        var createOrder = JsonConvert.DeserializeObject<PaymentStatusOutput>(responseCheckStatusOrder);
        var result = new CreatePaymentOrderInput
        {
            PaymentId = paymentId,
            Name = orderCache.FareRuleName,
            Description = createOrderRequest.Invoice.Description,
            UserId = userId,
            Price = createOrderRequest.Amount.Value,
            PaymentMonth = orderCache.Month,
            Currency = PaymentCurrencyEnum.RUB.ToString(),
            Created = DateTime.Parse(createOrder.Created),
            PaymentStatusSysName = createOrder.OrderStatus,
            PaymentStatusName = PaymentStatusEnum.Pending.GetEnumDescription()
        };

        return await Task.FromResult(result);
    }
    
    /// <summary>
    /// Метод устанавливает запросу заголовки авторизации.
    /// </summary>
    /// <returns>HttpClient для запроса в ПС.</returns>
    private async Task<HttpClient> SetPayMasterRequestAuthorizationHeader()
    {
        using var httpClient = new HttpClient();
        
        // Устанавливаем заголовки запросу.
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            _configuration["Commerce:PayMaster:ApiToken"]);

        return await Task.FromResult(httpClient);
    }

    /// <summary>
    /// Метод создает результат созданного заказа.
    /// </summary>
    /// <param name="order">Заказ.</param>
    /// <param name="orderCache">Заказ из кэша.</param>
    /// <param name="createOrderInput">Входная модель заказа.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="httpClient">Http-client.</param>
    /// <returns>Результирующая модель заказа.</returns>
    /// <exception cref="InvalidOperationException">Может бахнуть ошибку, если не прошла проверка статуса платежа в ПС.</exception>
    private async Task<CreateOrderOutput> CreatePaymentOrderResultAsync(CreateOrderOutput order,
        CreateOrderCache orderCache, CreateOrderInput createOrderInput, long userId, HttpClient httpClient)
    {
        // Проверяем статус заказа в ПС.
        var responseCheckStatusOrder = await httpClient
            .GetStringAsync(string.Concat(ApiConsts.CHECK_PAYMENT_STATUS, order.PaymentId));

        // Если ошибка получения данных платежа.
        if (string.IsNullOrEmpty(responseCheckStatusOrder))
        {
            var ex = new InvalidOperationException(
                "Ошибка проверки статуса платежа в ПС. " +
                $"Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
            throw ex;
        }
        
        var createdOrder = await CreatePaymentOrderRequestAsync(order.PaymentId, orderCache,
            createOrderInput.CreateOrderRequest, userId, responseCheckStatusOrder);

        // Создаем заказ в БД.
        var createdOrderResult = await _commerceRepository.CreateOrderAsync(createdOrder);
        
        var result = new CreateOrderOutput
        {
            PaymentId = createdOrderResult.OrderId.ToString(),
            Url = order.Url
        };

        return result;
    }

    #endregion
}