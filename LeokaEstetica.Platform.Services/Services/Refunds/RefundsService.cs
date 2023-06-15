using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
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
    private readonly IPayMasterService _payMasterService;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ICommerceService _commerceService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="ordersRepository">Репозиторий заказов.</param>
    /// <param name="refundsNotificationService">Сервис уведомлений возвратов.</param>
    /// <param name="payMasterService">Сервис возвратов в ПС.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    public RefundsService(ILogger<RefundsService> logger,
        ILogger<BaseCalculateRefundStrategy> loggerStrategy,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IOrdersRepository ordersRepository,
        IRefundsNotificationService refundsNotificationService,
        IPayMasterService payMasterService,
        IGlobalConfigRepository globalConfigRepository, 
        ICommerceService commerceService)
    {
        _logger = logger;
        _loggerStrategy = loggerStrategy;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _ordersRepository = ordersRepository;
        _refundsNotificationService = refundsNotificationService;
        _payMasterService = payMasterService;
        _globalConfigRepository = globalConfigRepository;
        _commerceService = commerceService;
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
                new CalculateRefundUsedDaysStrategy(_loggerStrategy, _commerceService), userId, orderId);

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
    public async Task<CreateRefundOutput> CreateRefundAsync(long orderId, decimal price, string account, string token)
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
            var refund = await _payMasterService.CreateRefundAsync(order.PaymentId, price,
                PaymentCurrencyEnum.RUB.ToString());

            var isReceiptRefund = await _globalConfigRepository.GetValueByKeyAsync<bool>(
                GlobalConfigKeys.Receipt.SEND_RECEIPT_REFUND_MODE_ENABLED);

            if (isReceiptRefund)
            {
                // Создаем модель запроса к ПС для создания чека возврата.
                var requestReceiptRefund = await CreateReceiptRefundRequestAsync(order, account, refund.RefundId,
                    refund.RefundOrderId);
                
                // Создаем чек возврата в ПС.
                _ = await _payMasterService.CreateReceiptRefundAsync(requestReceiptRefund);
            }

            return refund;
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

    /// <summary>
    /// TODO: В будущем возможно будет проблема с account, потому что ПС ожидает именно почту пользователя.
    /// TODO: а может придти не почта, а логин, тогда надо будет доработать такой кейс тут.
    /// Метод создает модель запроса в ПС для создания чека возврата.
    /// </summary>
    /// <param name="order">Данные заказа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="refundId">Id возврата.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <returns>Модель запроса в ПС.</returns>
    private async Task<CreateReceiptInput> CreateReceiptRefundRequestAsync(OrderEntity order, string account,
        long refundId, string refundOrderId)
    {
        var price = order.Price;
        var amount = new Amount(price, PaymentCurrencyEnum.RUB.ToString());
        var client = new ClientInput(account);

        // Создаем позиции чека.
        var items = new List<ReceiptItem>
        {
            new()
            {
                Name = order.OrderName,
                PaymentMethod = "FullPayment",
                PaymentSubject = "Service",
                VatType = VatTypeEnum.None.ToString(),
                Price = price,
                Quantity = 1
            }
        };

        var request = new CreateReceiptInput(order.PaymentId, amount, ReceiptTypeEnum.Refund.ToString(), client, items,
            order.OrderId, refundId, refundOrderId);

        return await Task.FromResult(request);
    }

    #endregion
}