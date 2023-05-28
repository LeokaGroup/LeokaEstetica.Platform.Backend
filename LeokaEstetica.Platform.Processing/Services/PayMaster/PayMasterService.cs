using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Helpers;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
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
    private readonly IRabbitMqService _rabbitMqService;

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
    /// <param name="rabbitMqService">Сервис кролика.</param>
    public PayMasterService(ILogService logService,
        IConfiguration configuration,
        IUserRepository userRepository,
        ICommerceRepository commerceRepository, 
        IAccessUserService accessUserService, 
        IAccessUserNotificationsService accessUserNotificationsService, 
        ICommerceRedisService commerceRedisService, 
        IRabbitMqService rabbitMqService)
    {
        _logService = logService;
        _configuration = configuration;
        _userRepository = userRepository;
        _commerceRepository = commerceRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
        _commerceRedisService = commerceRedisService;
        _rabbitMqService = rabbitMqService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<CreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
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
            var createOrderInput = await CreateOrderRequestAsync(orderCache.FareRuleName, orderCache.Price,
                orderCache.RuleId, publicId, orderCache.Month);

            await _logService.LogInfoAsync(new ApplicationException("Начало создания заказа."));

            using var httpClient = new HttpClient();
            await SetPayMasterRequestAuthorizationHeader(httpClient);
            
            // Создаем платеж в ПС.
            var responseCreateOrder = await httpClient.PostAsJsonAsync(ApiConsts.CREATE_PAYMENT,
                createOrderInput.CreateOrderRequest);

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

    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    public async Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId, HttpClient httpClient)
    {
        try
        {
            await _logService.LogInfoAsync(new ApplicationException($"Начало проверки статуса заказа {paymentId}."));
            
            await SetPayMasterRequestAuthorizationHeader(httpClient);
            
            var responseCreateOrder = await httpClient.GetAsync(string.Concat(ApiConsts.CHECK_PAYMENT_STATUS, 
                paymentId));
            
            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var ex = new InvalidOperationException("Ошибка проверки статуса платежа в ПС. " +
                                                       $"PaymentId платежа: {paymentId}");
                throw ex;
            }

            // Парсим результат из ПС.
            var order = await responseCreateOrder.Content.ReadFromJsonAsync<CheckStatusOrderOutput>();

            // Если ошибка при парсинге заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                var ex = new InvalidOperationException("Ошибка парсинга данных из ПС. " +
                                                       $"PaymentId платежа: {paymentId}");
                throw ex;
            }

            var result = PaymentStatus.GetPaymentStatusBySysName(order.StatusSysName);

            if (result == PaymentStatusEnum.None)
            {
                var ex = new InvalidOperationException("Неизвестный статус заказа." +
                                                       $"Статус заказа в ПС: {order.StatusSysName}." +
                                                       "Необходимо добавить маппинги для этого статуса заказа.");
                throw ex;
            }

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
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <param name="price">Цена.</param>
    /// <param name="ruleId">Id тарифа.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во мес, на которые оплачивается тариф.</param>
    /// <returns>Модель запроса в ПС PayMaster.</returns>
    private async Task<CreateOrderInput> CreateOrderRequestAsync(string fareRuleName, decimal price, int ruleId,
        Guid publicId, short month)
    {
        var result = new CreateOrderInput
        {
            CreateOrderRequest = new CreateOrderRequest
            {
                // Задаем Id мерчанта (магазина).
                MerchantId = new Guid(_configuration["Commerce:PayMaster:MerchantId"]),
                TestMode = true, // TODO: Добавить управляющий ключ в таблицу конфигов.
                Invoice = new Invoice
                {
                    Description = "Оплата тарифа: " + fareRuleName + $" (на {month} мес.)"
                },
                Amount = new Amount
                {
                    Value = price,
                    Currency = PaymentCurrencyEnum.RUB.ToString()
                },
                PaymentMethod = "BankCard",
                FareRuleId = ruleId
            },
            PublicId = publicId
        };

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод парсит результат для сохранения заказа в БД.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="orderCache">Заказ в кэше.</param>
    /// <param name="createOrderRequest">Модель запроса.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="responseCheckStatusOrder">Статус ответа из ПС.</param>
    /// <returns>Результирующая модель для сохранения в БД.</returns>
    private async Task<CreatePaymentOrderInput> CreatePaymentOrderAsync(string paymentId,
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
    private async Task SetPayMasterRequestAuthorizationHeader(HttpClient httpClient)
    {
        // Устанавливаем заголовки запросу.
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            _configuration["Commerce:PayMaster:ApiToken"]);

        await Task.CompletedTask;
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
        var paymentId = order.PaymentId;
        var responseCheckStatusOrder = await httpClient
            .GetStringAsync(string.Concat(ApiConsts.CHECK_PAYMENT_STATUS, paymentId));

        // Если ошибка получения данных платежа.
        if (string.IsNullOrEmpty(responseCheckStatusOrder))
        {
            var ex = new InvalidOperationException(
                "Ошибка проверки статуса платежа в ПС. " +
                $"Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
            throw ex;
        }
        
        var createdOrder = await CreatePaymentOrderAsync(paymentId, orderCache,
            createOrderInput.CreateOrderRequest, userId, responseCheckStatusOrder);

        // Создаем заказ в БД.
        var createdOrderResult = await _commerceRepository.CreateOrderAsync(createdOrder);
        
        // Отправляем заказ в очередь для отслеживания его статуса.
        var orderEvent = OrderEventFactory.CreateOrderEvent(createdOrderResult.OrderId,
            createdOrderResult.StatusSysName, paymentId);
        await _rabbitMqService.PublishAsync(orderEvent, QueueTypeEnum.OrdersQueue);
        
        var result = new CreateOrderOutput
        {
            PaymentId = paymentId,
            Url = order.Url
        };

        return result;
    }

    #endregion
}