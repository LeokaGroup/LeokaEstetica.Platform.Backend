using System.Net.Http.Json;
using System.Text;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Connection;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Factors;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Processing.Services.YandexKassa;

/// <summary>
/// Класс реализует методы сервиса платежной системы ЮKassa.
/// </summary>
internal sealed class YandexKassaService : IYandexKassaService
{
    private readonly ILogger<YandexKassaService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;
    private readonly ICommerceRedisService _commerceRedisService;
    private readonly IConfiguration _configuration;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ICommerceRepository _commerceRepository;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IMailingsService _mailingsService;
    private readonly ITransactionScopeFactory _transactionScopeFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="userRepository">Сервис доступов пользователей.</param>
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступов пользователей.</param>
    /// <param name="commerceRedisService">Сервис кэша коммерции.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="rabbitMqService">Сервис кролика.</param>
    /// <param name="mailingsService">Сервис email.</param>
    /// <param name="transactionScopeFactory">Факторка транзакций.</param> 
    public YandexKassaService(ILogger<YandexKassaService> logger,
        IUserRepository userRepository,
        IAccessUserService accessUserService,
        IAccessUserNotificationsService accessUserNotificationsService,
        ICommerceRedisService commerceRedisService,
        IConfiguration configuration,
        IGlobalConfigRepository globalConfigRepository,
        ICommerceRepository commerceRepository,
        IRabbitMqService rabbitMqService,
        IMailingsService mailingsService,
        ITransactionScopeFactory transactionScopeFactory)
    {
        _logger = logger;
        _userRepository = userRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
        _commerceRedisService = commerceRedisService;
        _configuration = configuration;
        _globalConfigRepository = globalConfigRepository;
        _commerceRepository = commerceRepository;
        _rabbitMqService = rabbitMqService;
        _mailingsService = mailingsService;
        _transactionScopeFactory = transactionScopeFactory;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
    {
        try
        {
            using var scope = _transactionScopeFactory.CreateTransactionScope();
            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                if (!string.IsNullOrEmpty(token))
                {
                    await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                        "Для покупки тарифа должна быть заполнена информация вашей анкеты.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);   
                }

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

            using var httpClient = new HttpClient().SetYandexKassaRequestAuthorizationHeader(_configuration);

            // Создаем платеж в ПС.
            var httpContent = new StringContent(JsonConvert.SerializeObject(createOrderInput.CreateOrderRequest),
                Encoding.UTF8, "application/json");
            var responseCreateOrder = await httpClient.PostAsync(new Uri(ApiConsts.YandexKassa.CREATE_PAYMENT),
                httpContent);

            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var responseErrorDetails = responseCreateOrder.Content.ReadAsStringAsync().Result;
                var ex = new InvalidOperationException(
                    "Ошибка создания платежа в ПС." +
                    $" Данные платежа: {JsonConvert.SerializeObject(createOrderInput.CreateOrderRequest)}." +
                    $" Ответ от ПС: {responseErrorDetails}");
                throw ex;
            }

            // Парсим результат из ПС.
            var mapOrder = await responseCreateOrder.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<CreateOrderYandexKassaOutput>(mapOrder);
            
            // Если ошибка при парсинге заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                var ex = new InvalidOperationException(
                    $"Ошибка парсинга данных из ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            var paymentOrderAggregateInput = CreatePaymentOrderAggregateInputResult(order, orderCache, createOrderInput,
                userId, publicId, fareRuleName, account, month);

            var result = await CreatePaymentOrderFactory.CreatePaymentOrderResultAsync(paymentOrderAggregateInput,
                    _configuration, _commerceRepository, _rabbitMqService, _globalConfigRepository, _mailingsService);

            _logger.LogInformation("Конец создания заказа.");
            
            scope.Complete();
            
            _logger.LogInformation("Создание заказа успешно.");

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            _logger.LogInformation($"Начало проверки статуса заказа {paymentId}.");
            
            using var scope = _transactionScopeFactory.CreateTransactionScope();
            using var httpClient = new HttpClient().SetYandexKassaRequestAuthorizationHeader(_configuration);

            var responseCreateOrder = await httpClient.GetAsync(string.Concat(ApiConsts.YandexKassa.CHECK_PAYMENT_STATUS,
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
            
            scope.Complete();
            
            _logger.LogInformation($"Закончили проверку статуса заказа {paymentId}.");

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="amount">Данные о цене.</param>
    public async Task ConfirmPaymentAsync(string paymentId, Amount amount)
    {
        try
        {
            _logger.LogInformation("Начало подтверждения заказа.");

            using var scope = _transactionScopeFactory.CreateTransactionScope();
            using var httpClient = new HttpClient().SetYandexKassaRequestAuthorizationHeader(_configuration);

            // Подтверждаем платеж в ПС и списываем ДС у пользователя.
            var httpContent = new StringContent(JsonConvert.SerializeObject(amount),
                Encoding.UTF8, "application/json");
            var responseConfirmOrder = await httpClient.PostAsync(
                new Uri($"{ApiConsts.YandexKassa.CONFIRM_PAYMENT}{paymentId}/capture"), httpContent);

            // Если ошибка при подтверждении заказа в ПС.
            if (!responseConfirmOrder.IsSuccessStatusCode)
            {
                var responseErrorDetails = responseConfirmOrder.Content.ReadAsStringAsync().Result;
                var ex = new InvalidOperationException(
                    "Ошибка подтверждения заказа в ПС." +
                    $" Данные заказа: {JsonConvert.SerializeObject(amount)}." +
                    $" Ответ от ПС: {responseErrorDetails}");
                throw ex;
            }

            await _commerceRepository.SetStatusConfirmByPaymentIdAsync(paymentId,
                PaymentStatusEnum.Succeeded.ToString(), PaymentStatusEnum.Succeeded.GetEnumDescription());
            
            scope.Complete();
            
            _logger.LogInformation("Закончили подтверждение заказа.");
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает модель запроса в ПС ЮKassa.
    /// </summary>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <param name="price">Цена.</param>
    /// <param name="ruleId">Id тарифа.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во мес, на которые оплачивается тариф.</param>
    /// <returns>Модель запроса в ПС PayMaster.</returns>
    private async Task<CreateOrderYandexKassaInput> CreateOrderRequestAsync(string fareRuleName, decimal price,
        int ruleId, Guid publicId, short month)
    {
        var isTestMode = await _globalConfigRepository.GetValueByKeyAsync<bool>(
            GlobalConfigKeys.Integrations.PaymentSystem.COMMERCE_TEST_MODE_ENABLED);

        var result = new CreateOrderYandexKassaInput
        {
            CreateOrderRequest = new CreateOrderYandexKassaRequest
            {
                TestMode = isTestMode,
                Description = "Оплата тарифа: " + fareRuleName + $" (на {month} мес.)",
                Amount = new Amount(price, PaymentCurrencyEnum.RUB.ToString()),
                PaymentMethodData = new PaymentMethodData("bank_card"),
                Confirmation = new Confirmation("redirect", "https://leoka-estetica.ru/return_url"),
                Metadata = new { FareRuleId = ruleId }
            },
            PublicId = publicId
        };

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод создает входную модель для создания заказа.
    /// </summary>
    /// <param name="order">Данные заказа.</param>
    /// <param name="orderCache">Данные заказа в кэше.</param>
    /// <param name="createOrderInput">Входная модель создания заказа.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="month">Кол-во месяцев, на которое оформлен тариф.</param>
    /// <returns>Наполненная входная модель для создания заказа.</returns>
    private CreatePaymentOrderAggregateInput CreatePaymentOrderAggregateInputResult(CreateOrderYandexKassaOutput order,
        CreateOrderCache orderCache, CreateOrderYandexKassaInput createOrderInput, long userId, Guid publicId,
        string fareRuleName, string account, short month)
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