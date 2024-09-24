using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.BuilderData;
using LeokaEstetica.Platform.Processing.Builders.Order;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Enum = System.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Processing.Services.Commerce;

/// <summary>
/// Класс реализует методы сервиса коммерции.
/// </summary>
internal sealed class CommerceService : ICommerceService
{
    private readonly ICommerceRedisService _commerceRedisService;
    private readonly ILogger<CommerceService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly ICommerceRepository _commerceRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IYandexKassaService _yandexKassaService;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// <param name="commerceRedisService">Сервис кэша коммерции.</param>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифов.</param>
    /// <param name="fareRuleRepository">Репозиторий коммерции.</param>
    /// <param name="ordersRepository">Репозиторий заказов.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    /// <param name="commerceService">Сервис проверки лимитов.</param>
    /// <param name="accessUserService">Сервис доступа пользователей.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="payMasterService">Сервис платежной системы PayMaster.</param>
    /// <param name="yandexKassaService">Сервис ЮKassa.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    /// <param name="mapper">Маппер.</param>
    /// </summary>
    public CommerceService(ICommerceRedisService commerceRedisService,
        ILogger<CommerceService> logger,
        IUserRepository userRepository,
        IFareRuleRepository fareRuleRepository,
        ICommerceRepository commerceRepository,
        IOrdersRepository ordersRepository,
        ISubscriptionRepository subscriptionRepository,
        IAccessUserService accessUserService,
        IGlobalConfigRepository globalConfigRepository,
        IYandexKassaService yandexKassaService,
        Lazy<IDiscordService> discordService,
        Lazy<IHubNotificationService> hubNotificationService,
        IMapper mapper)
    {
        _commerceRedisService = commerceRedisService;
        _logger = logger;
        _userRepository = userRepository;
        _fareRuleRepository = fareRuleRepository;
        _commerceRepository = commerceRepository;
        _ordersRepository = ordersRepository;
        _subscriptionRepository = subscriptionRepository;
        _accessUserService = accessUserService;
        _globalConfigRepository = globalConfigRepository;
        _yandexKassaService = yandexKassaService;
        _discordService = discordService;
        _hubNotificationService = hubNotificationService;
        _mapper = mapper;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<CreateOrderOutput> CreateOrderCacheOrRabbitMqAsync(CreateOrderInput createOrderInput,
        string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var orderBuilder = new OrderBuilder();
            BaseOrderBuilder? builder;
            (FareRuleCompositeOutput? FareRule, IEnumerable<FareRuleAttributeOutput>? FareRuleAttributes,
                IEnumerable<FareRuleAttributeValueOutput>? FareRuleAttributeValues) fareRule;
            
            fareRule.FareRule = default;
            fareRule.FareRuleAttributes = default;
            fareRule.FareRuleAttributeValues = default;

            // При оплате тарифа нам известен PublicId с фронта.
            if (createOrderInput.OrderType == OrderTypeEnum.FareRule)
            {
                fareRule = await GetFareRuleAsync(userId, createOrderInput.PublicId);
            }
            
            // При оплате вакансии мы не знаем на фронте PublicId.
            // Определим его исходя из текущего пользователя.
            else if (createOrderInput.OrderType == OrderTypeEnum.CreateVacancy)
            {
                var userSubscription = await _subscriptionRepository.GetUserSubscriptionByUserIdAsync(userId);

                if (userSubscription is null)
                {
                    throw new InvalidOperationException("Ошибка получения подписки пользователя. " +
                                                        $"UserId: {userId}.");
                }

                var publicId = (await _fareRuleRepository.GetByIdAsync(userSubscription.RuleId))?.PublicId;

                if (publicId is null || publicId == Guid.Empty)
                {
                    throw new InvalidOperationException("Ошибка получения публичного кода тарифа. " +
                                                        $"UserId: {userId}. " +
                                                        $"PublicId: {publicId}.");
                }
                
                fareRule = await GetFareRuleAsync(userId, publicId.Value);
            }

            var fareRuleAttributeValues = fareRule.FareRuleAttributeValues
                ?.FirstOrDefault(x => x.AttributeId == 4);

            switch (createOrderInput.OrderType)
            {
                case OrderTypeEnum.Undefined:
                    throw new InvalidOperationException(
                        "Неизвестный тип заказа. " +
                        $"CreateOrderInput: {JsonConvert.SerializeObject(createOrderInput)}.");

                // Создаем заказ на подписку тарифа.
                case OrderTypeEnum.FareRule:
                    builder = new FareRuleOrderBuilder(_subscriptionRepository, _commerceRepository)
                    {
                        OrderData = new OrderData
                        {
                            FareRuleAttributeValues = fareRuleAttributeValues,
                            FareRuleName = fareRule.FareRule!.RuleName,
                            CreatedBy = userId,
                            RuleId = fareRule.FareRule.RuleId,
                            OrderType = OrderTypeEnum.FareRule,
                            Month = createOrderInput.PaymentMonth,
                            EmployeesCount = createOrderInput.EmployeesCount,
                            PublicId = fareRule.FareRule.PublicId
                        }
                    };

                    await orderBuilder.BuildAsync(builder);

                    if (builder is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания билдера заказа тарифа. " +
                            $"CreateOrderInput: {JsonConvert.SerializeObject(createOrderInput)}.");
                    }

                    var ruleBuilder = builder as FareRuleOrderBuilder ??
                                      throw new InvalidCastException($"Ошибка каста к {nameof(FareRuleOrderBuilder)}");

                    if (ruleBuilder.OrderCache is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания модели заказа для сохранения его в кэше. " +
                            $"CreateOrderCacheInput: {JsonConvert.SerializeObject(createOrderInput)}.");
                    }
                    
                    if (ruleBuilder.OrderData is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания подготовительных данных заказа для сохранения его в кэше. " +
                            $"CreateOrderCacheInput: {JsonConvert.SerializeObject(createOrderInput)}.");
                    }

                    // Сохраняем заказ в кэш сроком на 4 часа.
                    var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId,
                        createOrderInput.PublicId);
                    await _commerceRedisService.CreateOrderCacheAsync(key, ruleBuilder.OrderCache);
                    
                    return _mapper.Map<CreateOrderOutput>(ruleBuilder.OrderCache);
                
                // В этом кейсе не добавляем в кэш.
                case OrderTypeEnum.CreateVacancy:
                    // Готовим билдер для создания заказа в ПС.
                    builder = new PostVacancyOrderBuilder(_subscriptionRepository, _commerceRepository)
                    {
                        OrderData = new OrderData
                        {
                            FareRuleAttributeValues = fareRuleAttributeValues,
                            FareRuleName = fareRule.FareRule!.RuleName,
                            CreatedBy = userId,
                            RuleId = fareRule.FareRule.RuleId,
                            OrderType = OrderTypeEnum.CreateVacancy,
                            Month = createOrderInput.PaymentMonth,
                            Account = account,
                            PublicId = fareRule.FareRule.PublicId
                        },
                        VacancyOrderData = createOrderInput.VacancyOrderData
                    };

                    await orderBuilder.BuildAsync(builder);

                    if (builder is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания билдера заказа на платную публикацию вакансии. " +
                            $"CreateOrderInput: {JsonConvert.SerializeObject(createOrderInput)}.");
                    }

                    var postVacancyBuilder = builder as PostVacancyOrderBuilder ??
                                             throw new InvalidCastException(
                                                 $"Ошибка каста к {nameof(PostVacancyOrderBuilder)}");

                    if (postVacancyBuilder.VacancyOrderData is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания модели вакансии для сохранения ее в очередь кролика. " +
                            "VacancyOrderData: " +
                            $"{JsonConvert.SerializeObject(createOrderInput.VacancyOrderData)}.");
                    }
                    
                    if (postVacancyBuilder.OrderData is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания модели данных для сохранения ее в очередь кролика и БД. " +
                            "VacancyOrderData: " +
                            $"{JsonConvert.SerializeObject(createOrderInput.VacancyOrderData)}.");
                    }
                    
                    if (postVacancyBuilder.OrderData.Amount is null)
                    {
                        throw new InvalidOperationException(
                            "Ошибка создания модели данных цены заказа для сохранения ее в очередь кролика и БД. " +
                            "VacancyOrderData: " +
                            $"{JsonConvert.SerializeObject(createOrderInput.VacancyOrderData)}.");
                    }
                    
                    // Создаем заказ в ПС и добавляем в нашу БД.
                    var order = await CreateOrderAsync(postVacancyBuilder);
                    
                    return _mapper.Map<CreateOrderOutput>(order);

                default:
                    throw new InvalidOperationException("Неизвестный кейс создания заказа. " +
                                                        $"OrderType: {createOrderInput.OrderType}.");
            }
        }

        catch (Exception ex)
        {
            _logger?.LogCritical(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<CreateOrderCache> GetOrderProductsCacheAsync(BaseOrderBuilder orderBuilder)
    {
        try
        {
            if (orderBuilder.OrderData is null)
            {
                throw new InvalidOperationException("Данные заказа не были подготовлены для получения услуг заказа.");
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(orderBuilder.OrderData.Account!);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(orderBuilder.OrderData.Account!);
                throw ex;
            }

            var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId, orderBuilder.OrderData.PublicId);
            var result = await _commerceRedisService.GetOrderCacheAsync(key);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод вычисляет сумму с оставшихся дней подписки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Сумма.</returns>
    public async Task<decimal> CalculatePriceSubscriptionFreeDaysAsync(long userId, long orderId)
    {
        // Вычисляем, сколько прошло дней использования подписки у пользователя.
        var usedDays = await _userRepository.GetUserSubscriptionUsedDateAsync(userId);

        // Если одна из дат пустая, то не можем вычислить сумму возврата. Возврат не делаем в таком кейсе.
        if (!usedDays.StartDate.HasValue || !usedDays.EndDate.HasValue)
        {
            _logger.LogWarning("Невозможно вычислить сумму возврата. Одна из дат подписки либо обе были null. " +
                               $"UserId: {userId}. " +
                               $"OrderId: {orderId}");
            return 0;
        }

        // Вычисляем кол-во дней, за которые можем учесть ДС пользователя при оплате новой подписки.
        var referenceUsedDays = (int)Math.Round(usedDays.EndDate.Value.Subtract(usedDays.StartDate.Value).TotalDays);

        // Получаем по какой цене был оформлен заказ.
        var orderPrice = (await _ordersRepository.GetOrderDetailsAsync(orderId, userId)).Price;

        // Узнаем цену подписки за день.
        var priceDay = Math.Round(orderPrice / referenceUsedDays);

        // Вычисляем сумму остатка (цена всей подписки - цена за день).
        var resultRefundPrice = Math.Round(orderPrice - priceDay);

        // Не можем вычислять остаток себе в ущерб.
        if (resultRefundPrice == 0)
        {
            _logger.LogWarning($"Невозможно сделать возврат на сумму: {resultRefundPrice}. Возврат не будет сделан." +
                               $"UserId: {userId}. " +
                               $"OrderId: {orderId}");
        }

        // Исключительная ситуация, сразу логируем такое.
        if (resultRefundPrice < 0)
        {
            _logger.LogError($"Сумма возврата была отрицательной: {resultRefundPrice}. Возврат не будет сделан." +
                             $"UserId: {userId}. " +
                             $"OrderId: {orderId}");
        }

        return resultRefundPrice;
    }

    /// <inheritdoc />
    public async Task<OrderFreeOutput> CheckFreePriceAsync(BaseOrderBuilder orderBuilder)
    {
        try
        {
            if (orderBuilder.OrderData is null)
            {
                throw new InvalidOperationException("Данные заказа не были подготовлены для проверки цены заказа.");
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(orderBuilder.OrderData.Account!);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(orderBuilder.OrderData.Account!);
                throw ex;
            }

            // Проверяем, есть ли у пользователя действующая платная подписка.
            var subscription = await _subscriptionRepository.GetUserSubscriptionByUserIdAsync(userId);

            if (subscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"Ошибка в {nameof(CommerceService)}");
            }

            var result = new OrderFreeOutput();

            // Находим Id заказа текущей подписки пользователя.
            var orderId = await _ordersRepository.GetUserOrderIdAsync(subscription.MonthCount.Value, userId);

            // Подписки нет, значит бесплатная. С бесплатной остатка не будет.
            if (orderId <= 0)
            {
                return result;
            }

            // TODO: Пока отключили скидку от остатка, так как это требует продумывания нашей системы ценообразования.
            // TODO: Пока не будем давать скидку от остатка.
            // Вычисляем остаток суммы подписки (пока без учета повышения/понижения подписки).
            // var freePrice = await CalculatePriceSubscriptionFreeDaysAsync(userId, orderId);

            // result.FreePrice = freePrice;

            // if (freePrice == 0)
            // {
            //     return result;
            // }

            // Проверяем повышение/понижение подписки.
            // Находим тариф.
            // var fareRule = await _fareRuleRepository.GetByPublicIdAsync(publicId);
            // var calcPrice = await CalculateServicePriceAsync(month, fareRule.Price);

            // Если сумма тарифа больше суммы остатка с текущей подписки пользователя,
            // то это и будет его выгода и мы учтем это при переходе на другую подписку.
            // if (calcPrice > freePrice)
            // {
            //     result.FreePrice = calcPrice - freePrice;
            // }

            // result.Price = calcPrice;

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет заполнение анкеты пользователя.
    /// Если не заполнена, то нельзя оформить заказ.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Признак результата проверки. False - Анкета заполнена. True - не заполнена.</returns>
    public async Task<bool> IsProfileEmptyAsync(string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        try
        {
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            // Если не заполнена, то не даем оформить платный тариф.
            if (isEmptyProfile)
            {
                var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Для оформления тарифа должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningEmptyUserProfile",
                    userCode, UserConnectionModuleEnum.Main);

                return true;
            }

            return false;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Анкета пользователя не заполнена. UserId был: {userId}");
            throw;
        }
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<ICreateOrderOutput> CreateOrderAsync(BaseOrderBuilder orderBuilder)
    {
        try
        {
            // Через какую ПС будем проводить оплату.
            var paymentSystemType = await _globalConfigRepository.GetValueByKeyAsync<string>(GlobalConfigKeys
                .Integrations.PaymentSystem.COMMERCE_PAYMENT_SYSTEM_TYPE_MODE);

            _logger.LogInformation($"Заказ будет создан в ПС: {paymentSystemType}.");

            var systemType = Enum.Parse<PaymentSystemEnum>(paymentSystemType);

            var paymentSystemJob = new PaymentSystemJob();

            _logger.LogInformation("Начали создание заказа в ПС.");

            // Создаем заказ в ПС, которая выбрана в конфиг таблице.
            var order = systemType switch
            {
                PaymentSystemEnum.Yandex => await paymentSystemJob.CreateOrderAsync(
                    new YandexKassaStrategy(_yandexKassaService), orderBuilder),

                // TODO: Не используется (в будущем возможно будет).
                // PaymentSystemEnum.PayMaster => await paymentSystemJob.CreateOrderAsync(
                //     new PayMasterStrategy(_payMasterService), orderBuilder),

                _ => throw new InvalidOperationException("Неизвестный тип платежной системы.")
            };

            _logger.LogInformation("Закончили создание заказа в ПС." +
                                   $" PaymentId: {order.PaymentId}." +
                                   $"Данные заказа: {JsonConvert.SerializeObject(order)}.");

            return order;
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
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Статус платежа.</returns>
    public async Task<PaymentStatusEnum> CheckOrderStatusAsync(BaseOrderBuilder orderBuilder)
    {
        var paymentSystemType = await _globalConfigRepository.GetValueByKeyAsync<string>(GlobalConfigKeys
            .Integrations.PaymentSystem.COMMERCE_PAYMENT_SYSTEM_TYPE_MODE);
        _logger.LogInformation($"Проверка статуса заказа в ПС: {paymentSystemType}.");

        var systemType = Enum.Parse<PaymentSystemEnum>(paymentSystemType);
        var paymentSystemJob = new PaymentSystemJob();

        // Проверяем статус заказа в ПС, которая выбрана в конфиг таблице.
        var orderStatus = systemType switch
        {
            PaymentSystemEnum.Yandex => await paymentSystemJob.CheckOrderStatusAsync(
                new YandexKassaStrategy(_yandexKassaService), orderBuilder),

             // TODO: Не используется (в будущем возможно будет).
            // PaymentSystemEnum.PayMaster => await paymentSystemJob.CheckOrderStatusAsync(
            //     new PayMasterStrategy(_payMasterService), orderBuilder),

            _ => throw new InvalidOperationException("Неизвестный тип платежной системы.")
        };

        return orderStatus;
    }

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    public async Task ConfirmPaymentAsync(BaseOrderBuilder orderBuilder)
    {
        var paymentSystemType = await _globalConfigRepository.GetValueByKeyAsync<string>(GlobalConfigKeys
            .Integrations.PaymentSystem.COMMERCE_PAYMENT_SYSTEM_TYPE_MODE);
        _logger.LogInformation($"Подтверждение платежа в ПС: {paymentSystemType}.");

        var systemType = Enum.Parse<PaymentSystemEnum>(paymentSystemType);
        var paymentSystemJob = new PaymentSystemJob();

        // Подтверждаем платеж в ПС, которая выбрана в конфиг таблице.
        // После этого будут списаны ДС.
        if (systemType == PaymentSystemEnum.Yandex)
        {
            await paymentSystemJob.ConfirmPaymentAsync(
                new YandexKassaStrategy(_yandexKassaService), orderBuilder);
        }

        else
        {
            throw new InvalidOperationException("Неизвестный тип платежной системы.");
        }
    }

    /// <inheritdoc />
    public async Task<CalculateFareRulePriceOutput> CalculateFareRulePriceAsync(Guid publicId, int selectedMonth,
        int employeeCount, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var fareRule = await GetFareRuleAsync(userId, publicId);

            var fareRuleAttributeValues = fareRule.FareRuleAttributeValues?.FirstOrDefault(x => x.AttributeId == 4);

            var result = new CalculateFareRulePriceOutput
            {
                // Цена тарифа = минимальная цена тарифа * кол-во сотрудников * кол-во месяцев.
                Price = Math.Round(fareRuleAttributeValues!.MinValue!.Value * employeeCount * selectedMonth),
                IsNeedUserAction = true
            };

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<CalculatePostVacancyPriceOutput> CalculatePricePostVacancyAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var userSubscription = await _subscriptionRepository.GetUserSubscriptionByUserIdAsync(userId);

            if (userSubscription is null)
            {
                throw new InvalidOperationException("Ошибка получения подписки пользователя. " +
                                                    $"UserId: {userId}.");
            }

            // Получаем услугу по тарифу.
            var fees = await _commerceRepository.GetFeesByFareRuleIdAsync(userSubscription.RuleId);

            if (fees is null)
            {
                throw new InvalidOperationException("Ошибка получения услуги. " +
                                                    $"UserId: {userId}. " +
                                                    $"RuleId: {userSubscription.RuleId}.");
            }

            var result = new CalculatePostVacancyPriceOutput
            {
                Price = fees.FeesPrice,
                IsNeedUserAction = true,
                Fees = fees
            };

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// TODO: Когда проведем аналитику скидок, то внедрим в новую версию оплат.
    /// Метод получает скидку на услугу по ее типу и кол-ву месяцев.
    /// </summary>
    /// <param name="paymentMonth">Кол-во месяцев.</param>
    /// <param name="discountTypeEnum">Тип скидки на услугу</param>
    /// <returns>Скидка на услугу.</returns>
    private async Task<decimal> GetPercentDiscountAsync(short paymentMonth, DiscountTypeEnum discountTypeEnum)
    {
        var result = await _commerceRepository.GetPercentDiscountAsync(paymentMonth, discountTypeEnum);

        return result;
    }

    /// <summary>
    /// TODO: Пока не используем, но в будущем внедрим скидки и это будет нужно.
    /// Метод вычисляет сумму с учетом скидки.
    /// Если цена null.
    /// </summary>
    /// <param name="percent">% скидки.</param>
    /// <param name="price">Сумму без скидки.</param>
    /// <returns>Сумма с учетом скидки.</returns>
    private async Task<decimal> CalculatePercentPriceAsync(decimal percent, decimal price)
    {
        // Если нет скидки, то оставляем цену такой же.
        if (percent == 0)
        {
            return price;
        }

        return await Task.FromResult(price - Math.Round(price * percent / 100));
    }

    /// <summary>
    /// Метод получает детали тарифа.
    /// Все необходимые валидации проводятся внутри.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="employeesCount">Кол-во сотрудников организации.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Кортеж с данными тарифа.</returns>
    /// <exception cref="InvalidOperationException">Если не прошли валидацию.</exception>
    private async Task<(FareRuleCompositeOutput? FareRule, IEnumerable<FareRuleAttributeOutput>? FareRuleAttributes,
        IEnumerable<FareRuleAttributeValueOutput>? FareRuleAttributeValues)> GetFareRuleAsync(long userId,
        Guid publicId)
    {
        // Получаем тариф, на который пользователь пытается перейти.
        var newFareRule = await _fareRuleRepository.GetFareRuleByPublicIdAsync(publicId);

        if (newFareRule.FareRule is null)
        {
            throw new InvalidOperationException($"Не удалось получить новый тариф пользователя. UserId: {userId}");
        }

        var fareRuleAttributeValues = newFareRule.FareRuleAttributeValues?.FirstOrDefault(x => x.AttributeId == 4);

        // Обязательно должна быть минимальная цена у тарифа.
        if (fareRuleAttributeValues?.MinValue is null)
        {
            throw new InvalidOperationException(
                "У тарифа обнаружено отсутствие минимальной цены. " +
                "У нас у тарифов пока минимальная цена, это есть фиксированная цена тарифа. " +
                $"Требуется обработка кейса. Id тарифа: {newFareRule.FareRule.RuleId}.");
        }

        // Логируем ошибку, но не ломаем приложение, так как мы пока не закладывали такой кейс.
        if (fareRuleAttributeValues.MinValue.HasValue && fareRuleAttributeValues.MaxValue.HasValue)
        {
            var ex = new InvalidOperationException(
                "У тарифа обнаружен диапазон цен. Такой кейс пока не закладывали, но он сработал. " +
                "У нас у тарифов пока лишь минимальная цена есть. " +
                $"Требуется обработка кейса. Id тарифа: {newFareRule.FareRule.RuleId}.");

            await _discordService.Value.SendNotificationErrorAsync(ex);

            _logger?.LogError(ex, ex.Message);
        }

        return newFareRule;
    }

    #endregion
}