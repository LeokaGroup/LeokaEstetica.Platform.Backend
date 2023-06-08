using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Refunds;
using LeokaEstetica.Platform.Services.Strategies.Refunds;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Refunds;

/// <summary>
/// Класс реализует методы сервиса возвратов в нашей системе.
/// </summary>
public sealed class RefundsService : IRefundsService
{
    private readonly ILogger<RefundsService> _logger;
    private readonly ILogger<BaseCalculateRefundStrategy> _loggerStrategy;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IRefundsNotificationService _refundsNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="ordersRepository">Репозиторий заказов.</param>
    /// <param name="refundsNotificationService">Сервис уведомлений возвратов.</param>
    public RefundsService(ILogger<RefundsService> logger,
        ILogger<BaseCalculateRefundStrategy> loggerStrategy,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IOrdersRepository ordersRepository,
        IRefundsNotificationService refundsNotificationService)
    {
        _logger = logger;
        _loggerStrategy = loggerStrategy;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _ordersRepository = ordersRepository;
        _refundsNotificationService = refundsNotificationService;
    }

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
}