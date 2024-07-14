using System.Runtime.CompilerServices;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
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
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Common.Cache.Output;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IPayMasterService _payMasterService;
    private readonly IMapper _mapper;
    private readonly IYandexKassaService _yandexKassaService;
    private readonly Lazy<IDiscordService> _discordService;

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
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступа пользователей.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="payMasterService">Сервис платежной системы PayMaster.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="yandexKassaService">Сервис ЮKassa.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// </summary>
    public CommerceService(ICommerceRedisService commerceRedisService,
        ILogger<CommerceService> logger,
        IUserRepository userRepository,
        IFareRuleRepository fareRuleRepository,
        ICommerceRepository commerceRepository,
        IOrdersRepository ordersRepository,
        ISubscriptionRepository subscriptionRepository,
        IAccessUserService accessUserService,
        IAccessUserNotificationsService accessUserNotificationsService,
        IGlobalConfigRepository globalConfigRepository,
        IPayMasterService payMasterService,
        IMapper mapper,
        IYandexKassaService yandexKassaService,
        Lazy<IDiscordService> discordService)
    {
        _commerceRedisService = commerceRedisService;
        _logger = logger;
        _userRepository = userRepository;
        _fareRuleRepository = fareRuleRepository;
        _commerceRepository = commerceRepository;
        _ordersRepository = ordersRepository;
        _subscriptionRepository = subscriptionRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
        _globalConfigRepository = globalConfigRepository;
        _payMasterService = payMasterService;
        _mapper = mapper;
        _yandexKassaService = yandexKassaService;
        _discordService = discordService;
    }

    #region Публичные методы.

    // <summary>
    /// Метод создает заказ в кэше.
    /// </summary>
    /// <param name="createOrderCache">Модель заказа для хранения в кэше.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные заказа добавленного в кэш.</returns>
    public async Task<CreateOrderCacheOutput> CreateOrderCacheAsync(CreateOrderCacheInput createOrderCacheInput,
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

            // Получаем тариф, на который пользователь пытается перейти.
            var newFareRule = await _fareRuleRepository.GetFareRuleByPublicIdAsync(createOrderCacheInput.PublicId);
            
            if (newFareRule is null)
            {
                throw new InvalidOperationException($"Не удалось получить новый тариф пользователя. UserId: {userId}");
            }
            
            // Обязательно должна быть минимальная цена у тарифа.
            if (!newFareRule.MinValue.HasValue)
            {
                throw new InvalidOperationException(
                    "У тарифа обнаружено отсутствие минимальной цены. " +
                    "У нас у тарифов пока минимальная цена, это есть фиксированная цена тарифа. " +
                    $"Требуется обработка кейса. Id тарифа: {newFareRule.RuleId}.");
            }
            
            // Логируем ошибку, но не ломаем приложение, так как мы пока не закладывали такой кейс.
            if (newFareRule.MinValue.HasValue && newFareRule.MaxValue.HasValue)
            {
                var ex = new InvalidOperationException(
                    "У тарифа обнаружен диапазон цен. Такой кейс пока не закладывали, но он сработал. " +
                    "У нас у тарифов пока лишь минимальная цена есть. " +
                    $"Требуется обработка кейса. Id тарифа: {newFareRule.RuleId}.");

                await _discordService.Value.SendNotificationErrorAsync(ex);

                _logger?.LogError(ex, ex.Message);
            }
            
            var result = new CreateOrderCacheOutput
            {
                Errors = new List<ValidationFailure>(),
                // Цена тарифа = минимальная цена тарифа * кол-во сотрудников * кол-во месяцев.
                Price = newFareRule.MinValue.Value * createOrderCacheInput.EmployeesCount *
                        createOrderCacheInput.PaymentMonth
            };

            // Если не было подтверждения действия от пользователя.
            if (!createOrderCacheInput.IsCompleteUserAction)
            {
                // Проверяем, вмещает ли тариф кол-во сотрудников, которое хочет пользователь.
                var isCorrectEmployeesCnt = await _fareRuleRepository.CheckAvailableEmployeesCountFareRuleAsync(
                    createOrderCacheInput.PublicId, createOrderCacheInput.EmployeesCount);

                // Предлагаем сменить тариф, так как цена выросла.
                // Тариф не вмещает кол-во сотрудников - будет изменение цены за рамки цены тарифа.
                if (!isCorrectEmployeesCnt)
                {
                    result.Errors.Add(new ValidationFailure
                    {
                        Severity = Severity.Warning,
                        ErrorMessage = "Выбранный тариф не вмещает в себя указанное количество сотрудников. " +
                                       "Выберите другой тариф."
                    });
                }

                result.IsNeedUserAction = true;
                result.IsNeedChangeFareRule = true;

                return result;
            }

            var order = new CreateOrderCache
            {
                RuleId = newFareRule.RuleId,
                Month = createOrderCacheInput.PaymentMonth,
                UserId = userId,
                FareRuleName = newFareRule.FareRuleName,
                Price = result.Price
            };

            // Сохраняем заказ в кэш сроком на 2 часа.
            var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId, createOrderCacheInput.PublicId);
            await _commerceRedisService.CreateOrderCacheAsync(key, order);

            result = _mapper.Map<CreateOrderCacheOutput>(order);
            result.IsNeedUserAction = false;
            result.IsNeedChangeFareRule = false;

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogCritical(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает услуги и сервисы заказа из кэша.
    /// </summary>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Услуги и сервисы заказа.</returns>
    public async Task<CreateOrderCache> GetOrderProductsCacheAsync(Guid publicId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var key = await _commerceRedisService.CreateOrderCacheKeyAsync(userId, publicId);
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

    /// <summary>
    /// TODO: Скорректировать логику под новую версию оплат.
    /// Метод вычисляет, есть ли остаток с прошлой подписки пользователя для учета ее как скидку при оформлении новой подписки.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <returns>Сумма остатка, если она есть.</returns>
    public async Task<OrderFreeOutput> CheckFreePriceAsync(string account, Guid publicId, short month)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Проверяем, есть ли у пользователя действующая платная подписка.
            var subscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);

            if (subscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"Ошибка в {nameof(CommerceService)}");
            }

            var subscriptionId = subscription.SubscriptionId;
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionBySubscriptionIdAsync(
                subscriptionId, userId);

            if (userSubscription is null)
            {
                throw new InvalidOperationException("Не удалось получить подписку пользователя." +
                                                    $"UserId: {userId}." +
                                                    $"SubscriptionId: {subscriptionId}");
            }

            var result = new OrderFreeOutput();

            // Находим Id заказа текущей подписки пользователя.
            var orderId = await _ordersRepository.GetUserOrderIdAsync(userSubscription.MonthCount, userId);

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
            var fareRule = await _fareRuleRepository.GetByPublicIdAsync(publicId);
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
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Признак результата проверки. False - Анкета заполнена. True - не заполнена.</returns>
    public async Task<bool> IsProfileEmptyAsync(string account, string token)
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
                await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                    "Для оформления тарифа должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);

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
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
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
                    new YandexKassaStrategy(_yandexKassaService), publicId, account, token),

                PaymentSystemEnum.PayMaster => await paymentSystemJob.CreateOrderAsync(
                    new PayMasterStrategy(_payMasterService), publicId, account, token),

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
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    public async Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId)
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
                new YandexKassaStrategy(_yandexKassaService), paymentId),

            PaymentSystemEnum.PayMaster => await paymentSystemJob.CheckOrderStatusAsync(
                new PayMasterStrategy(_payMasterService), paymentId),

            _ => throw new InvalidOperationException("Неизвестный тип платежной системы.")
        };

        return orderStatus;
    }

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="amount">Данные о цене.</param>
    public async Task ConfirmPaymentAsync(string paymentId, Amount amount)
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
                new YandexKassaStrategy(_yandexKassaService), paymentId, amount);
        }

        else
        {
            throw new InvalidOperationException("Неизвестный тип платежной системы.");
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

    #endregion
}