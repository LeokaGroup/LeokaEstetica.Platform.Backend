using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Database.Abstractions.Config;
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
using LeokaEstetica.Platform.Processing.Models;
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
    public YandexKassaService(ILogger<YandexKassaService> logger,
        IUserRepository userRepository,
        IAccessUserService accessUserService,
        IAccessUserNotificationsService accessUserNotificationsService,
        ICommerceRedisService commerceRedisService,
        IConfiguration configuration,
        IGlobalConfigRepository globalConfigRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
        _commerceRedisService = commerceRedisService;
        _configuration = configuration;
        _globalConfigRepository = globalConfigRepository;
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

            using var httpClient = new HttpClient();
            await SetYandexKassaRequestAuthorizationHeader(httpClient);
            
            // Создаем платеж в ПС.
            var responseCreateOrder = await httpClient.PostAsJsonAsync(ApiConsts.YandexKassa.CREATE_PAYMENT,
                createOrderInput.CreateOrderRequest);

            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var ex = new InvalidOperationException(
                    $"Ошибка создания платежа в ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }

            // Парсим результат из ПС.
            var order = await responseCreateOrder.Content.ReadFromJsonAsync<CreateOrderYandexKassaOutput>();
            
            // Если ошибка при парсинге заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                var ex = new InvalidOperationException(
                    $"Ошибка парсинга данных из ПС. Данные платежа: {JsonConvert.SerializeObject(createOrderInput)}");
                throw ex;
            }
            
            var paymentOrderAggregateInput = CreatePaymentOrderAggregateInputResult(order, orderCache,
                createOrderInput, userId, httpClient, publicId, fareRuleName, account, month);
            
            var reference = AutoFac.Resolve<IPaymentOrderReference>();
            var result = await CreatePaymentOrderFactory.CreatePaymentOrderResultAsync(paymentOrderAggregateInput,
                reference);

            _logger.LogInformation("Конец создания заказа.");
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
        using var httpClient = new HttpClient();
        await SetYandexKassaRequestAuthorizationHeader(httpClient);
            
        _logger?.LogInformation($"Начало проверки статуса заказа {paymentId}.");

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
                                                   $"Статус заказа в ПС: {order.StatusSysName}." +
                                                   "Необходимо добавить маппинги для этого статуса заказа.");
            throw ex;
        }

        return result;
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
                ShopId = Convert.ToInt32(_configuration["Commerce:UKassa:ShopId"]),
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
    /// Метод устанавливает запросу заголовки авторизации.
    /// </summary>
    private async Task SetYandexKassaRequestAuthorizationHeader(HttpClient httpClient)
    {
        // Устанавливаем заголовки запросу.
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            _configuration["Commerce:UKassa:ShopId"] + ":" + _configuration["Commerce:UKassa:ApiToken"]);

        await Task.CompletedTask;
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
    private CreatePaymentOrderAggregateInput CreatePaymentOrderAggregateInputResult(CreateOrderYandexKassaOutput order,
        CreateOrderCache orderCache, CreateOrderYandexKassaInput createOrderInput, long userId, HttpClient httpClient,
        Guid publicId, string fareRuleName, string account, short month)
    {
        var reesult = new CreatePaymentOrderAggregateInput
        {
            CreateOrderOutput = order,
            CreateOrderInput = createOrderInput,
            OrderCache = orderCache,
            UserId = userId,
            HttpClient = httpClient,
            PublicId = publicId,
            FareRuleName = fareRuleName,
            Account = account,
            Month = month
        };

        return reesult;
    }

    #endregion
}