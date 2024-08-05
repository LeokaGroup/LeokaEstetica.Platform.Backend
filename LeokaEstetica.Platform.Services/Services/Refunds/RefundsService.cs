using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Refunds;
using LeokaEstetica.Platform.Services.Strategies.Refunds;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
    private readonly IPayMasterService _payMasterService;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ICommerceService _commerceService;
    private readonly ICommerceRepository _commerceRepository;
    private readonly IMapper _mapper;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="ordersRepository">Репозиторий заказов.</param>
    /// <param name="payMasterService">Сервис возвратов в ПС.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    public RefundsService(ILogger<RefundsService> logger,
        ILogger<BaseCalculateRefundStrategy> loggerStrategy,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IOrdersRepository ordersRepository,
        IPayMasterService payMasterService,
        IGlobalConfigRepository globalConfigRepository,
        ICommerceService commerceService,
        ICommerceRepository commerceRepository,
        IMapper mapper,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _loggerStrategy = loggerStrategy;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _ordersRepository = ordersRepository;
        _payMasterService = payMasterService;
        _globalConfigRepository = globalConfigRepository;
        _commerceService = commerceService;
        _commerceRepository = commerceRepository;
        _mapper = mapper;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод вычисляет сумму возврата заказа.
    /// Возврат делается только за неиспользованный период подписки.
    /// <param name="orderId">Id заказа.</param>
    /// <param name="account">Аккаунт.</param>
    /// </summary>
    /// <returns>Выходная модель.</returns>
    public async Task<CalculateRefundOutput> CalculateRefundAsync(string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            throw new NotFoundUserIdByAccountException(account);
        }
        
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
        try
        {
            // Получаем подписку.
            var currentSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            if (currentSubscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"Ошибка в {nameof(RefundsService)}");
            }

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
                await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                    "Ошибка при вычислении суммы возврата. Мы уже знаем о проблеме и уже занимаемся ей. " +
                    $"Вы можете обратиться в тех.поддержку. ID вашего заказа {orderId}",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorCalculateRefund", userCode,
                    UserConnectionModuleEnum.Processing);

                throw new InvalidOperationException("Не удалось вычислить сумму возврата. " +
                                                    $"OrderId: {orderId}. " +
                                                    $"UserId: {userId}");
            }

            var price = result.Price;

            if (price <= 0)
            {
                await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                    "Ошибка при вычислении суммы возврата. Мы уже знаем о проблеме и уже занимаемся ей. " +
                    $"Вы можете обратиться в тех.поддержку. ID вашего заказа {orderId}",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorCalculateRefund", userCode,
                    UserConnectionModuleEnum.Processing);

                throw new InvalidOperationException("Сумма возврата не может быть отрицательной." +
                                                    $"Price: {price}" +
                                                    $"OrderId: {orderId}. " +
                                                    $"UserId: {userId}");
            }

            return result;
        }

        catch (Exception ex)
        {
            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при вычислении суммы возврата. Мы уже знаем о проблеме и уже занимаемся ей. ",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorCalculateRefund", userCode,
                UserConnectionModuleEnum.Processing);

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
    /// <returns>Выходная модель.</returns>
    public async Task<CreateRefundOutput> CreateRefundAsync(long orderId, decimal price, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            throw new NotFoundUserIdByAccountException(account);
        }
        
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
        try
        {
            // Получаем данные для возврата из заказа.
            var order = await _ordersRepository.GetOrderDetailsAsync(orderId, userId);

            if (order is null)
            {
                var ex = new InvalidOperationException("Не удалось получить детали заказа. " +
                                                       $"OrderId: {orderId}." +
                                                       $"UserId: {userId}");
                throw ex;
            }

            var isReceiptRefund = await _globalConfigRepository.GetValueByKeyAsync<bool>(
                GlobalConfigKeys.Receipt.SEND_RECEIPT_REFUND_MODE_ENABLED);
            CreateRefundOutput refund;

            // Автоматическое создание возврата в ПС.
            if (isReceiptRefund)
            {
                /// TODO: Доработать этот метод для работы с разными ПС через стратегию, по аналогии как работают платежи.
                // Создаем возврат в ПС.
                refund = await _payMasterService.CreateRefundAsync(order.PaymentId, price,
                    PaymentCurrencyEnum.RUB.ToString());
                
                // Создаем модель запроса к ПС для создания чека возврата.
                var requestReceiptRefund = await CreateReceiptRefundRequestAsync(order, account, refund.RefundId,
                    refund.RefundOrderId);
                
                // Создаем чек возврата в ПС.
                _ = await _payMasterService.CreateReceiptRefundAsync(requestReceiptRefund);
            }

            // Ручное создание возврата.
            else
            {
                _logger.LogInformation("Начали ручное создание возврата." +
                                       $" Данные заказа для возврата: {JsonConvert.SerializeObject(order)}");
                
                // Проверяем, создан ли уже такой возврат.
                var isExists = await _commerceRepository.IfExistsRefundAsync(orderId.ToString());

                if (isExists)
                {
                    await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                        "Такой возврат уже создан. Нельзя создать возврат повторно." +
                        $"ID вашего заказа по которому сделан возврат {orderId}",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningManualRefund",
                        userCode, UserConnectionModuleEnum.Processing);

                    return null;
                }
                
                // TODO: PaymentId пока всегда ставим null. отому что возвраты пока создаем только вручную.
                // TODO: Также проставляем признак ручного создания возврата IsManual.
                // Создаем возврат в БД.
                var createdRefund = await _commerceRepository.CreateRefundAsync(null, price,
                    DateTime.UtcNow, RefundStatusEnum.Pending.ToString(), orderId.ToString(), true);

                refund = _mapper.Map<CreateRefundOutput>(createdRefund);

                // TODO: Доработать тут логику, чтобы создавать вручную чек к возврату как возврат прихода (54-ФЗ).
                
                _logger.LogInformation("Закончили ручное создание возврата." +
                                       $" Данные заказа для возврата: {JsonConvert.SerializeObject(order)}");
                _logger.LogInformation($"Заказ {orderId} ожидает ручной обработки возврата.");
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Возврат записан и будет обработан. Как только мы обработаем возврат," +
                " Вы получите уведомление об этом на почту. " +
                $"ID вашего заказа по которому будет возврат {orderId}",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessManualRefund", userCode,
                UserConnectionModuleEnum.Processing);

            return refund;
        }

        catch (Exception ex)
        {
            _logger?.LogCritical(ex.Message, ex);

            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при возврате. Мы уже знаем о проблеме и уже занимаемся ей. " +
                $"Вы можете обратиться в тех.поддержку. ID вашего заказа {orderId}",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorRefund", userCode,
                UserConnectionModuleEnum.Processing);

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
    private async Task<CreateReceiptPayMasterInput> CreateReceiptRefundRequestAsync(OrderEntity order, string account,
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

        var request = new CreateReceiptPayMasterInput(order.PaymentId, amount, ReceiptTypeEnum.Refund.ToString(),
            client, items, order.OrderId, refundId, refundOrderId);

        return await Task.FromResult(request);
    }

    #endregion
}