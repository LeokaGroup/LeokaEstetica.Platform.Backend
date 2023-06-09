using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Refunds;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Refunds;
using LeokaEstetica.Platform.Services.Strategies.Refunds;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Refunds;

/// <summary>
/// Класс реализует методы сервиса возвратов в нашей системе.
/// </summary>
internal sealed class RefundsService : IRefundsService
{
    private readonly ILogger<RefundsService> _logger;
    private readonly ILogger<BaseCalculateRefundStrategy> _loggerStrategy;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IRefundsNotificationService _refundsNotificationService;
    private readonly IRefundsRepository _refundsRepository;
    private readonly IPayMasterService _payMasterService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="ordersRepository">Репозиторий заказов.</param>
    /// <param name="refundsNotificationService">Сервис уведомлений возвратов.</param>
    /// <param name="refundsRepository">Репозиторий возвратов.</param>
    /// <param name="payMasterService">Сервис возвратов в ПС.</param>
    public RefundsService(ILogger<RefundsService> logger,
        ILogger<BaseCalculateRefundStrategy> loggerStrategy,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IOrdersRepository ordersRepository,
        IRefundsNotificationService refundsNotificationService, 
        IRefundsRepository refundsRepository, 
        IPayMasterService payMasterService)
    {
        _logger = logger;
        _loggerStrategy = loggerStrategy;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _ordersRepository = ordersRepository;
        _refundsNotificationService = refundsNotificationService;
        _refundsRepository = refundsRepository;
        _payMasterService = payMasterService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод вычисляет сумму возврата заказа.
    /// Возврат делается только за неиспользованный период подписки.
    /// <param name="orderId">Id заказа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// </summary>
    /// <returns>Выходная модель.</returns>
    public async Task<CalculateRefundOutput> CalculateRefundAsync(string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }
            
            // Получаем подписку.
            var currentSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository
                .GetUserSubscriptionBySubscriptionIdAsync(currentSubscription.SubscriptionId, userId);

            // Получаем заказ, который был оформлен на подписку.
            var orderId = await _ordersRepository.GetUserOrderIdAsync(userSubscription.MonthCount, userId);
            
            if (orderId <= 0)
            {
                var ex = new InvalidOperationException($"Id заказа был <= 0. OrderId: {orderId}");
                throw ex;
            }

            var calculateRefund = new CalculateRefund();
            var result = await calculateRefund.CalculateRefundAsync(
                new CalculateRefundUsedDaysStrategy(_loggerStrategy, _userRepository, _ordersRepository), userId,
                orderId);

            if (result is null)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await _refundsNotificationService.SendNotificationErrorCalculateRefundAsync("Что то пошло не так",
                        "Ошибка при вычислении суммы возврата. Мы уже знаем о проблеме и уже занимаемся ей. " +
                        $"Вы можете обратиться в тех.поддержку. ID вашего заказа {orderId}",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                }

                var ex = new InvalidOperationException("Не удалось вычислить сумму возврата. " +
                                                       $"OrderId: {orderId}. " +
                                                       $"UserId: {userId}");
                throw ex;
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод создает возврат по заказу.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Выходная модель.</returns>
    public async Task<RefundEntity> CreateRefundAsync(long orderId, decimal price, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }
            
            // Получаем данные для возврата из заказа.
            var order = await _ordersRepository.GetOrderDetailsAsync(orderId, userId);

            if (order is null)
            {
                var ex = new InvalidOperationException("Не удалось получить детали заказа. " +
                                                       $"OrderId: {orderId}." +
                                                       $"UserId: {userId}");
                throw ex;
            }

            // Создаем возврат в ПС.
            var paymentId = order.PaymentId;
            var refund = await _payMasterService.CreateRefundAsync(paymentId, price,
                PaymentCurrencyEnum.RUB.ToString());

            // Сохраняем возврат в БД.
            var result = await _refundsRepository.SaveRefundAsync(paymentId, price, refund.DateCreated, refund.Status,
                refund.RefundId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogCritical(ex.Message, ex);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _refundsNotificationService.SendNotificationErrorRefundAsync("Что то пошло не так",
                    "Ошибка при возврате. Мы уже знаем о проблеме и уже занимаемся ей. " +
                    $"Вы можете обратиться в тех.поддержку. ID вашего заказа {orderId}",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }
            
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}