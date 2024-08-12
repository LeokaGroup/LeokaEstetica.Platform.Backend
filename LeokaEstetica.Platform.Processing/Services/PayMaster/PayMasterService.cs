using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Factors;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Processing.Services.PayMaster;

/// <summary>
/// TODO: Не используем пока что PayMaster.
/// Класс реализует методы сервиса работы с платежной системой PayMaster.
/// </summary>
internal sealed class PayMasterService : IPayMasterService
{
    private readonly ILogger<PayMasterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly ICommerceRepository _commerceRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly ICommerceRedisService _commerceRedisService;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IMapper _mapper;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IMailingsService _mailingsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="configuration">Конфигурация внедренная через DI.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="accessUserService">Сервис доступа пользователя.</param>
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступа пользователя.</param>
    /// <param name="commerceRedisService">Сервис кэша коммерции.</param>
    /// <param name="rabbitMqService">Сервис кролика.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="mailingsService">Сервис email.</param>
    public PayMasterService(ILogger<PayMasterService> logger,
        IConfiguration configuration,
        IUserRepository userRepository,
        ICommerceRepository commerceRepository, 
        IAccessUserService accessUserService,
        ICommerceRedisService commerceRedisService, 
        IRabbitMqService rabbitMqService, 
        IMapper mapper,
        IGlobalConfigRepository globalConfigRepository,
        IMailingsService mailingsService)
    {
        _logger = logger;
        _configuration = configuration;
        _userRepository = userRepository;
        _commerceRepository = commerceRepository;
        _accessUserService = accessUserService;
        _commerceRedisService = commerceRedisService;
        _rabbitMqService = rabbitMqService;
        _mapper = mapper;
        _globalConfigRepository = globalConfigRepository;
        _mailingsService = mailingsService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account)
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

                // if (!string.IsNullOrEmpty(token))
                // {
                //     await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                //         "Для покупки тарифа должна быть заполнена информация вашей анкеты.",
                //         NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);   
                // }

                throw ex;
            }
            
            // Получаем заказ из кэша.
            var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId, publicId);
            var orderCache = await _commerceRedisService.GetOrderCacheAsync(key);
            
            // Заполняем модель для запроса в ПС.
            var fareRuleName = orderCache.FareRuleName;
            var month = orderCache.Month;
            var createOrderInput = await CreateOrderRequestAsync(fareRuleName, orderCache.Price,
                orderCache.RuleId, publicId, month);

            _logger.LogInformation("Начало создания заказа.");

            using var httpClient = new HttpClient();
            await SetPayMasterRequestAuthorizationHeader(httpClient);
            
            // Создаем платеж в ПС.
            var responseCreateOrder = await httpClient.PostAsJsonAsync(ApiConsts.PayMaster.CREATE_PAYMENT,
                createOrderInput.CreateOrderRequest);

            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var ex = new InvalidOperationException(
                    $"Ошибка создания платежа в ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            // Парсим результат из ПС.
            var order = await responseCreateOrder.Content.ReadFromJsonAsync<CreateOrderPayMasterOutput>();

            // Если ошибка при парсинге заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                var ex = new InvalidOperationException(
                    $"Ошибка парсинга данных из ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            var paymentOrderAggregateInput = CreatePaymentOrderAggregateInputResult(order, orderCache,
                createOrderInput, userId, httpClient, publicId, fareRuleName, account, month);
            
            var result = await CreatePaymentOrderFactory.CreatePaymentOrderResultAsync(paymentOrderAggregateInput,
                _configuration, _commerceRepository, _rabbitMqService, _globalConfigRepository, _mailingsService);

            _logger.LogInformation("Конец создания заказа.");
            _logger.LogInformation("Создание заказа успешно.");

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    public async Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId)
    {
        try
        {
            using var httpClient = new HttpClient();
            await SetPayMasterRequestAuthorizationHeader(httpClient);
            
            _logger?.LogInformation($"Начало проверки статуса заказа {paymentId}.");

            var responseCreateOrder = await httpClient.GetAsync(string.Concat(ApiConsts.PayMaster.CHECK_PAYMENT_STATUS,
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
                                                       $" Статус заказа в ПС: {order.StatusSysName}." +
                                                       " Необходимо добавить маппинги для этого статуса заказа.");
                throw ex;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// TODO: Доработать этот метод для работы с разными ПС через стратегию, по аналогии как работают платежи.
    /// Метод создает возврат в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="currency">Валюта.</param>
    /// <returns>Выходная модель.</returns>
    public async Task<CreateRefundOutput> CreateRefundAsync(string paymentId, decimal price, string currency)
    {
        var request = await CreateRefundRequestAsync(paymentId, price, currency);

        using var httpClient = new HttpClient();
        await SetPayMasterRequestAuthorizationHeader(httpClient);
        
        _logger?.LogInformation($"Начало создания возврата платежа {paymentId}. Сумма к возврату: {price}");
        
        // Создаем возврат в ПС.
        var responseCreateRefund = await httpClient.PostAsJsonAsync(ApiConsts.PayMaster.CREATE_REFUND, request);

        // Если ошибка при возврате платежа в ПС.
        if (!responseCreateRefund.IsSuccessStatusCode)
        {
            var ex = new InvalidOperationException(
                "Ошибка возврата платежа в ПС. " +
                "Стоит проверить статус платежа в ПС, проверить сумму возврата платежа, " +
                "проверить дату создания платежа в ПС." +
                $"Данные возврата: {JsonConvert.SerializeObject(request)}");
            throw ex;
        }

        var refund = await CreateRefundResultAsync(responseCreateRefund, request);
        
        // Создаем возврат в БД.
        var createdRefund = await _commerceRepository.CreateRefundAsync(refund.PaymentId, refund.Amount.Value,
            refund.DateCreated, refund.Status, refund.RefundOrderId, false);

        var result = _mapper.Map<CreateRefundOutput>(createdRefund);
        
        // Отправляем возврат в очередь для отслеживания его статуса.
        var refundEvent = RefundEventFactory.CreateRefundEvent(createdRefund.RefundId, createdRefund.PaymentId,
            createdRefund.Status, createdRefund.RefundOrderId);

        var queueType = string.Empty.CreateQueueDeclareNameFactory(_configuration["Environment"],
            QueueTypeEnum.RefundsQueue);
        // await _rabbitMqService.PublishAsync(refundEvent, queueType);

        _logger?.LogInformation("Конец создания возврата платежа.");

        return result;
    }

    /// <summary>
    /// Метод проверяет статус возврата в ПС.
    /// </summary>
    /// <param name="refundId">Id возврата.</param>
    /// <param name="httpClient">HttpClient.</param>
    /// <returns>Статус возврата.</returns>
    public async Task<RefundStatusEnum> CheckRefundStatusAsync(string refundId, HttpClient httpClient)
    {
        try
        {
            await SetPayMasterRequestAuthorizationHeader(httpClient);
            
            _logger?.LogInformation($"Начало проверки статуса возврата {refundId}.");

            var responseCreatedRefund = await httpClient.GetAsync(
                string.Concat(ApiConsts.PayMaster.CHECK_REFUND_STATUS, refundId));
            
            // Если ошибка при проверки статуса возврата в ПС.
            if (!responseCreatedRefund.IsSuccessStatusCode)
            {
                var ex = new InvalidOperationException("Ошибка проверки статуса возврата в ПС. " +
                                                       $"RefundId возврата: {refundId}");
                throw ex;
            }

            // Парсим результат из ПС.
            var refund = await responseCreatedRefund.Content.ReadFromJsonAsync<CheckStatusRefundOutput>();

            // Если ошибка при парсинге возврата из ПС.
            if (string.IsNullOrEmpty(refund?.RefundId))
            {
                var ex = new InvalidOperationException("Ошибка парсинга данных из ПС. " +
                                                       $"RefundId возврата: {refundId}");
                throw ex;
            }

            var result = RefundStatus.GetPaymentStatusBySysName(refund.StatusSysName);

            if (result == RefundStatusEnum.None)
            {
                var ex = new InvalidOperationException("Неизвестный статус возврата." +
                                                       $"Статус возврата в ПС: {refund.StatusSysName}." +
                                                       "Необходимо добавить маппинги для этого статуса возврата.");
                throw ex;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод создает чек возврата в ПС и отправляет его пользователю на почту.
    /// <param name="createReceiptInput">Входная модель.</param>
    /// </summary>
    /// <returns>Выходная модель чека.</returns>
    public async Task<CreateReceiptOutput> CreateReceiptRefundAsync(CreateReceiptPayMasterInput createReceiptInput)
    {
        using var httpClient = new HttpClient();
        await SetPayMasterRequestAuthorizationHeader(httpClient);
        
        _logger?.LogInformation($"Начало создания чека возврата по платежу {createReceiptInput.PaymentId}. " +
                                $"Сумма чека возврата: {createReceiptInput.Amount.Value}");
        
        // Создаем чек возврата в ПС.
        var response = await httpClient.PostAsJsonAsync(ApiConsts.PayMaster.CREATE_RECEIPT, createReceiptInput);

        // Если ошибка при возврате платежа в ПС.
        if (!response.IsSuccessStatusCode)
        {
            var ex = new InvalidOperationException(
                "Ошибка создания чека возврата в ПС. " +
                $"Данные возврата: {JsonConvert.SerializeObject(createReceiptInput)}");
            throw ex;
        }
            
        var refund = await CreateReceiptRefundResultAsync(response, createReceiptInput);

        // Создаем чек возврата в БД.
        var createdRefund = await _commerceRepository.CreateReceiptRefundAsync(refund);

        var result = _mapper.Map<CreateReceiptOutput>(createdRefund);
        
        // Отправляем чек возврата в очередь для отслеживания его статуса.
        var receiptRefundEvent = ReceiptRefundEventFactory.CreateReceiptRefundEvent(refund.ReceiptId,
            refund.PaymentId, refund.Status);

        var queueType = string.Empty.CreateQueueDeclareNameFactory(_configuration["Environment"],
            QueueTypeEnum.ReceiptRefundQueue);
        // await _rabbitMqService.PublishAsync(receiptRefundEvent, queueType);

        _logger?.LogInformation("Конец создания чека возврата платежа.");

        return result;
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
    private async Task<CreateOrderPayMasterInput> CreateOrderRequestAsync(string fareRuleName, decimal price,
        int ruleId, Guid publicId, short month)
    {
        var isTestMode = await _globalConfigRepository.GetValueByKeyAsync<bool>(
                    GlobalConfigKeys.Integrations.PaymentSystem.COMMERCE_TEST_MODE_ENABLED);
        
        var result = new CreateOrderPayMasterInput
        {
            CreateOrderRequest = new CreateOrderPayMasterRequest
            {
                // Задаем Id мерчанта (магазина).
                MerchantId = new Guid(_configuration["Commerce:PayMaster:MerchantId"]),
                TestMode = isTestMode,
                Invoice = new InvoicePayMaster
                {
                    Description = "Оплата тарифа: " + fareRuleName + $" (на {month} мес.)"
                },
                Amount = new Amount(price, PaymentCurrencyEnum.RUB.ToString()),
                PaymentMethod = "BankCard",
                FareRuleId = ruleId
            },
            PublicId = publicId
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
    /// Метод создает модель запроса в ПС для возврата.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="currency">Валюта.</param>
    /// <returns>Заполненная модель запроса.</returns>
    private async Task<CreateRefundInput> CreateRefundRequestAsync(string paymentId, decimal price,
        string currency)
    {
        var request = new CreateRefundInput
        {
            PaymentId = paymentId,
            Amount = new Amount(price, currency)
        };

        return await Task.FromResult(request);
    }

    /// <summary>
    /// Метод создает результат возврата заказа.
    /// </summary>
    /// <param name="responseCreateRefund">Данные возврата из ПС.</param>
    /// <param name="request">Модель запроса в ПС.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Может бахнуть ошибку, если не получилось распарсить результат из ПС.</exception>
    private async Task<CreateRefundOutput> CreateRefundResultAsync(HttpResponseMessage responseCreateRefund,
        CreateRefundInput request)
    {
        // Парсим результат из ПС.
        var refund = await responseCreateRefund.Content.ReadFromJsonAsync<CreateRefundOutput>();

        // Если ошибка при парсинге возврата из ПС, то не даем создать возврат.
        if (string.IsNullOrEmpty(refund?.PaymentId))
        {
            var ex = new InvalidOperationException(
                $"Ошибка парсинга данных из ПС. Данные возврата: {JsonConvert.SerializeObject(request)}");
            throw ex;
        }

        return refund;
    }
    
    /// <summary>
    /// Метод создает результат создания чека возврата.
    /// </summary>
    /// <param name="response">Данные создания чека возврата из ПС.</param>
    /// <param name="createReceiptInput">Модель запроса в ПС.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Может бахнуть ошибку, если не получилось распарсить результат из ПС.</exception>
    private async Task<CreateReceiptOutput> CreateReceiptRefundResultAsync(HttpResponseMessage response,
        CreateReceiptPayMasterInput createReceiptInput)
    {
        // Парсим результат из ПС.
        var refund = await response.Content.ReadFromJsonAsync<CreateReceiptOutput>();

        // Если ошибка при парсинге чека возврата из ПС, то не даем создать чек возврата.
        if (string.IsNullOrEmpty(refund?.PaymentId))
        {
            var ex = new InvalidOperationException(
                "Ошибка парсинга данных из ПС. " +
                $"Данные чека возврата: {JsonConvert.SerializeObject(createReceiptInput)}");
            throw ex;
        }

        refund.RefundId = createReceiptInput.RefundId;
        refund.OrderId = createReceiptInput.OrderId;

        return refund;
    }

    /// <summary>
    /// Метод создает входную модель для создания заказа.
    /// </summary>
    /// <param name="order">Данные заказа.</param>
    /// <param name="orderCache">Данные заказа в кэше.</param>
    /// <param name="createOrderInput">Входная модель создания заказа.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="httpClient">Клиент http-запросов.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="month">Кол-во месяцев, на которое оформлен тариф.</param>
    /// <returns>Наполненная входная модель для создания заказа.</returns>
    private CreatePaymentOrderAggregateInput CreatePaymentOrderAggregateInputResult(CreateOrderPayMasterOutput order,
        CreateOrderCache orderCache, CreateOrderPayMasterInput createOrderInput, long userId, HttpClient httpClient,
        Guid publicId, string fareRuleName, string account, short month)
    {
        var reesult = new CreatePaymentOrderAggregateInput
        {
            CreateOrderOutput = order,
            CreateOrderInput = createOrderInput,
            OrderCache = orderCache,
            UserId = userId,
            PublicId = publicId,
            FareRuleName = fareRuleName,
            Account = account,
            Month = month
        };

        return reesult;
    }

    #endregion
}