using System.Net.Http.Json;
using System.Text;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Builders.Order;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
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
    private readonly ICommerceRedisService _commerceRedisService;
    private readonly IConfiguration _configuration;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ICommerceRepository _commerceRepository;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IMailingsService _mailingsService;
    private readonly ITransactionScopeFactory _transactionScopeFactory;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;
    private readonly IOrdersService _ordersService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="userRepository">Сервис доступов пользователей.</param>
    /// <param name="commerceRedisService">Сервис кэша коммерции.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="rabbitMqService">Сервис кролика.</param>
    /// <param name="mailingsService">Сервис email.</param>
    /// <param name="transactionScopeFactory">Факторка транзакций.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    /// <param name="ordersService">Сервис заказов.</param>
    public YandexKassaService(ILogger<YandexKassaService> logger,
        IUserRepository userRepository,
        IAccessUserService accessUserService,
        ICommerceRedisService commerceRedisService,
        IConfiguration configuration,
        IGlobalConfigRepository globalConfigRepository,
        ICommerceRepository commerceRepository,
        IRabbitMqService rabbitMqService,
        IMailingsService mailingsService,
        ITransactionScopeFactory transactionScopeFactory,
        Lazy<IHubNotificationService> hubNotificationService,
        IOrdersService ordersService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _accessUserService = accessUserService;
        _commerceRedisService = commerceRedisService;
        _configuration = configuration;
        _globalConfigRepository = globalConfigRepository;
        _commerceRepository = commerceRepository;
        _rabbitMqService = rabbitMqService;
        _mailingsService = mailingsService;
        _transactionScopeFactory = transactionScopeFactory;
        _hubNotificationService = hubNotificationService;
        _ordersService = ordersService;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<ICreateOrderOutput> CreateOrderAsync(BaseOrderBuilder orderBuilder)
    {
        try
        {
            if (orderBuilder.OrderData is null)
            {
                throw new InvalidOperationException("Данные заказа не были подготовлены для оплаты заказа.");
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(orderBuilder.OrderData.Account!);

            if (userId <= 0)
            {
                throw new InvalidOperationException("Ошибка определения пользователя при создании заказа. " +
                                                    $"UserId: {userId}.");
            }
            
            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Для покупки тарифа должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningEmptyUserProfile",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }
            
            using var scope = _transactionScopeFactory.CreateTransactionScope();

            string? description = null;
            string? fareRuleName = null;
            short? month = null;
            VacancyInput? vacancy = null;
            int ruleId = 0;
            decimal price = 0;
            CreateOrderCache? orderCache = null;
            
            if (orderBuilder.OrderData.OrderType == OrderTypeEnum.FareRule)
            {
                // Получаем заказ из кэша.
                var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId, orderBuilder.OrderData.PublicId);
                orderCache = await _commerceRedisService.GetOrderCacheAsync(key);
                
                month = orderCache.Month;
                fareRuleName = orderCache.FareRuleName;
                description = "Оплата тарифа: " + fareRuleName + $" (на {month} мес.)";
                price = orderCache.Price;
                ruleId = orderCache.RuleId;
                
                var isTestPayMode = await _globalConfigRepository.GetValueByKeyAsync<bool>(
                    GlobalConfigKeys.Integrations.PaymentSystem.COMMEFCE_TEST_PRICE_MODE_ENABLED);

                // Если хотим провести тестовый платеж, но на реальных ДС.
                if (isTestPayMode)
                {
                    var testPayPrice = await _globalConfigRepository.GetValueByKeyAsync<decimal>(GlobalConfigKeys
                        .Integrations.PaymentSystem.COMMERCE_TEST_PRICE_MODE_ENABLED_VALUE);
                    orderCache.Price = testPayPrice;
                }
            }
            
            else if (orderBuilder.OrderData.OrderType == OrderTypeEnum.CreateVacancy)
            {
                fareRuleName = orderBuilder.OrderData.FareRuleName;
                description = "Оплата тарифа: " + fareRuleName;
                
                if (orderBuilder.OrderData.Amount is null)
                {
                    throw new InvalidOperationException(
                        "Ошибка определения данных о цене заказа. " +
                        $"OrderData: {JsonConvert.SerializeObject(orderBuilder.OrderData)}.");
                }
                
                // TODO: Доработать под кейс с бесплатным заказом. В ПС такой нет смысла оформлять.
                // TODO: Просто позволять создать вакансию надо, но с фиксацией такого заказа в нашей БД.
                if (orderBuilder.OrderData.Amount.Value <= 0)
                {
                    throw new InvalidOperationException(
                        "Цена заказа не может быть отрицательной и меньше нуля. " +
                        $"OrderData: {JsonConvert.SerializeObject(orderBuilder.OrderData)}.");
                }
                
                price = orderBuilder.OrderData.Amount.Value;

                if (!orderBuilder.OrderData.RuleId.HasValue)
                {
                    throw new InvalidOperationException(
                        "Ошибка определения Id тарифа. " +
                        $"OrderData: {JsonConvert.SerializeObject(orderBuilder.OrderData)}.");
                }
                
                ruleId = orderBuilder.OrderData.RuleId.Value;
                
                vacancy = ((PostVacancyOrderBuilder)orderBuilder).VacancyOrderData;
            }

            // Заполняем модель для запроса в ПС.
            // Готовим запрос в ПС.
            var createOrderInput = await CreateOrderRequestAsync(price, ruleId, orderBuilder.OrderData.PublicId,
                description);

            // TODO: Переделать на факторку HttpClientFactory.
            using var httpClient = new HttpClient().SetYandexKassaRequestAuthorizationHeader(_configuration);

            // Создаем платеж в ПС.
            // Создание платежа в ПС происходит за 1 этап (без холдирования).
            // Так как мы не включали его. Мы просто сразу хотим списывать ДС.
            var httpContent = new StringContent(JsonConvert.SerializeObject(createOrderInput.CreateOrderRequest),
                Encoding.UTF8, "application/json");
            
            _logger.LogInformation("Начало создания заказа.");
            
            var responseCreateOrder = await httpClient.PostAsync(new Uri(ApiConsts.YandexKassa.CREATE_PAYMENT),
                httpContent);

            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var responseErrorDetails = await responseCreateOrder.Content.ReadAsStringAsync();
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

            var paymentOrderAggregateInput = CreatePaymentOrderAggregateInputResult(order, orderCache,
                createOrderInput, userId, orderBuilder.OrderData.PublicId, fareRuleName,
                orderBuilder.OrderData.Account!, month);
            paymentOrderAggregateInput.PublicId = orderBuilder.OrderData.PublicId;

            var result = await _ordersService.CreatePaymentOrderAsync(paymentOrderAggregateInput,
                _configuration, _commerceRepository, _rabbitMqService, _globalConfigRepository, _mailingsService,
                vacancy, orderBuilder.OrderData.OrderType);

            _logger.LogInformation("Конец создания заказа.");
            
            scope.Complete();
            
            _logger.LogInformation("Создание заказа успешно.");

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PaymentStatusEnum> CheckOrderStatusAsync(BaseOrderBuilder orderBuilder)
    {
        try
        {
            if (orderBuilder.OrderData is null)
            {
                throw new InvalidOperationException("Данные заказа не были подготовлены для проверки статуса заказа.");
            }
            
            _logger.LogInformation($"Начало проверки статуса заказа {orderBuilder.OrderData.PaymentId}.");
            
            using var scope = _transactionScopeFactory.CreateTransactionScope();
            using var httpClient = new HttpClient().SetYandexKassaRequestAuthorizationHeader(_configuration);

            var responseCreateOrder = await httpClient.GetAsync(string.Concat(
                ApiConsts.YandexKassa.CHECK_PAYMENT_STATUS, orderBuilder.OrderData.PaymentId));
            
            // Если ошибка при создании платежа в ПС.
            if (!responseCreateOrder.IsSuccessStatusCode)
            {
                var ex = new InvalidOperationException("Ошибка проверки статуса платежа в ПС. " +
                                                       $"PaymentId платежа: {orderBuilder.OrderData.PaymentId}");
                throw ex;
            }

            // Парсим результат из ПС.
            var order = await responseCreateOrder.Content.ReadFromJsonAsync<CheckStatusOrderOutput>();

            // Если ошибка при парсинге заказа из ПС, то не даем создать заказ.
            if (string.IsNullOrEmpty(order?.PaymentId))
            {
                var ex = new InvalidOperationException("Ошибка парсинга данных из ПС. " +
                                                       $"PaymentId платежа: {orderBuilder.OrderData.PaymentId}");
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
            
            _logger.LogInformation($"Закончили проверку статуса заказа {orderBuilder.OrderData.PaymentId}.");

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task ConfirmPaymentAsync(BaseOrderBuilder orderBuilder)
    {
        try
        {
            if (orderBuilder.OrderData is null)
            {
                throw new InvalidOperationException("Данные заказа не были подготовлены для подтверждения заказа.");
            }
            
            _logger.LogInformation("Начало подтверждения заказа.");

            using var scope = _transactionScopeFactory.CreateTransactionScope();
            using var httpClient = new HttpClient().SetYandexKassaRequestAuthorizationHeader(_configuration);

            // Подтверждаем платеж в ПС и списываем ДС у пользователя.
            var httpContent = new StringContent(JsonConvert.SerializeObject(orderBuilder.OrderData.Amount),
                Encoding.UTF8, "application/json");
            
            var responseConfirmOrder = await httpClient.PostAsync(
                new Uri($"{ApiConsts.YandexKassa.CONFIRM_PAYMENT}{orderBuilder.OrderData.PaymentId}/capture"),
                httpContent);

            // Если ошибка при подтверждении заказа в ПС.
            if (!responseConfirmOrder.IsSuccessStatusCode)
            {
                var responseErrorDetails = responseConfirmOrder.Content.ReadAsStringAsync().Result;
                var ex = new InvalidOperationException(
                    "Ошибка подтверждения заказа в ПС. " +
                    $" Данные заказа: {JsonConvert.SerializeObject(orderBuilder)}. " +
                    $" Ответ от ПС: {responseErrorDetails}.");
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(orderBuilder.OrderData.PaymentId))
            {
                throw new InvalidOperationException(
                    "PaymentId не был заполнен. " +
                    $"OrderData: {JsonConvert.SerializeObject(orderBuilder.OrderData)}.");
            }

            await _commerceRepository.SetStatusConfirmByPaymentIdAsync(orderBuilder.OrderData.PaymentId,
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
    /// Метод создает модель запроса в ПС.
    /// </summary>
    /// <param name="price">Цена.</param>
    /// <param name="ruleId">Id тарифа.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="description">Описание заказа.</param>
    /// <returns>Модель запроса в ПС.</returns>
    private async Task<CreateOrderYandexKassaInput> CreateOrderRequestAsync(decimal price,
        int ruleId, Guid publicId, string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new InvalidOperationException("Не заполнено описание заказа.");
        }
        
        var isTestMode = await _globalConfigRepository.GetValueByKeyAsync<bool>(
            GlobalConfigKeys.Integrations.PaymentSystem.COMMERCE_TEST_MODE_ENABLED);

        var result = new CreateOrderYandexKassaInput
        {
            CreateOrderRequest = new CreateOrderYandexKassaRequest
            {
                TestMode = isTestMode,
                Description = description,
                Amount = new Amount(price, CurrencyTypeEnum.RUB.ToString()),
                PaymentMethodData = new PaymentMethodData("bank_card"),
                Confirmation = new Confirmation("redirect", "https://leoka-estetica.ru/return_url"),
                Metadata = new { FareRuleId = ruleId }
            },
            PublicId = publicId
        };

        return await Task.FromResult(result);
    }

    /// <summary>
    /// TODO: Засунуть параметры в одну модель - их уже слишком много тут.
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
        CreateOrderCache? orderCache, CreateOrderYandexKassaInput createOrderInput, long userId, Guid publicId,
        string? fareRuleName, string account, short? month)
    {
        var reesult = new CreatePaymentOrderAggregateInput
        {
            CreateOrderOutput = order,
            CreateOrderInput = createOrderInput,
            OrderCache = orderCache,
            CreatedBy = userId,
            PublicId = publicId,
            FareRuleName = fareRuleName,
            Account = account,
            Month = month
        };

        return reesult;
    }

    #endregion
}