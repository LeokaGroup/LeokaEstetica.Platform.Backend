using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.Base;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.Processing.Models.Output;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Processing.Services.Commerce;

/// <summary>
/// Класс реализует методы сервиса заказов.
/// </summary>
internal sealed class OrdersService : IOrdersService
{
    private readonly ILogger<OrdersService> _logger;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="ordersRepository">Репозиторий заказов пользователя.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public OrdersService(ILogger<OrdersService> logger,
        IOrdersRepository ordersRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _ordersRepository = ordersRepository;
        _userRepository = userRepository;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<OrderEntity>> GetUserOrdersAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = await _ordersRepository.GetUserOrdersAsync(userId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<OrderEntity> GetOrderDetailsAsync(long orderId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = await _ordersRepository.GetOrderDetailsAsync(orderId, userId);

            if (result is null)
            {
                var ex = new InvalidOperationException($"Не удалось получить детали заказа. Id заказа: {orderId}");
                throw ex;
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HistoryEntity>> GetHistoryAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = await _ordersRepository.GetHistoryAsync(userId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ICreateOrderOutput> CreatePaymentOrderAsync(
        CreatePaymentOrderAggregateInput createPaymentOrderAggregateInput, IConfiguration configuration,
        ICommerceRepository commerceRepository, IRabbitMqService rabbitMqService,
        IGlobalConfigRepository globalConfigRepository, IMailingsService mailingsService, VacancyInput? vacancy,
        OrderTypeEnum orderType)
    {
        try
        {
            var paymentId = string.Empty;
            ICreateOrderOutput? result = null;

            // TODO: Переделать на факторку.
            using var httpClient = new HttpClient();

            if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderPayMasterOutput payMasterOrder)
            {
                paymentId = payMasterOrder.PaymentId;
            }

            else if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderYandexKassaOutput yandexOrder)
            {
                paymentId = yandexOrder.PaymentId;
                result = yandexOrder;

                httpClient.SetYandexKassaRequestAuthorizationHeader(configuration);
            }

            // Проверяем статус заказа в ПС.
            var responseCheckStatusOrder = await httpClient.GetStringAsync(
                string.Concat(ApiConsts.YandexKassa.CHECK_PAYMENT_STATUS, paymentId));

            // Если ошибка получения данных платежа.
            if (string.IsNullOrEmpty(responseCheckStatusOrder))
            {
                var ex = new InvalidOperationException(
                    "Ошибка проверки статуса платежа в ПС. " +
                    $"Данные платежа: {JsonConvert.SerializeObject(createPaymentOrderAggregateInput.CreateOrderInput)}");
                throw ex;
            }

            var createdOrder = await CreatePaymentOrderAsync(paymentId,
                createPaymentOrderAggregateInput.CreateOrderInput, createPaymentOrderAggregateInput.CreatedBy,
                responseCheckStatusOrder, orderType, createPaymentOrderAggregateInput.FareRuleName,
                createPaymentOrderAggregateInput.Month);

            // Создаем заказ в БД.
            var createdOrderResult = await commerceRepository.CreateOrderAsync(createdOrder);

            if (createdOrderResult is null)
            {
                throw new InvalidOperationException("Ошибка создания заказа в БД. " +
                                                    $"СreatedOrder: {JsonConvert.SerializeObject(createdOrder)}.");
            }

            OrderEvent? orderEvent = null;
            
            if (createdOrderResult.OrderType == OrderTypeEnum.FareRule)
            {
                /// TODO: Засунуть все параметры в модель. Их слишком много уже тут.
                // Отправляем заказ в очередь для отслеживания его статуса.
                orderEvent = OrderEventFactory.CreateOrderEvent(createdOrderResult.OrderId,
                    createdOrderResult.StatusSysName, paymentId, createPaymentOrderAggregateInput.CreatedBy,
                    createPaymentOrderAggregateInput.PublicId, createPaymentOrderAggregateInput.OrderCache.Month,
                    createdOrderResult.Price, createdOrder.CurrencyType, createdOrderResult.OrderType);
            }
            
            else if (createdOrderResult.OrderType == OrderTypeEnum.CreateVacancy)
            {
                /// TODO: Засунуть все параметры в модель. Их слишком много уже тут.
                // Отправляем заказ в очередь для отслеживания его статуса.
                orderEvent = OrderEventFactory.CreatePostVacancyOrderEvent(createdOrderResult.OrderId,
                    createdOrderResult.StatusSysName, paymentId, createPaymentOrderAggregateInput.CreatedBy,
                    createPaymentOrderAggregateInput.PublicId, createPaymentOrderAggregateInput.Month,
                    createdOrderResult.Price, createdOrder.CurrencyType, vacancy!,
                    createdOrderResult.OrderType);
            }

            var queueType = string.Empty.CreateQueueDeclareNameFactory(configuration["Environment"],
                QueueTypeEnum.OrdersQueue);

            if (orderEvent is null)
            {
                throw new InvalidOperationException("Ошибка формирования ивента для кролика.");
            }

            await rabbitMqService.PublishAsync(orderEvent, queueType, configuration,
                QueueTypeEnum.OrdersQueue | QueueTypeEnum.OrdersQueue).ConfigureAwait(false);

            var isEnabledEmailNotifications = await globalConfigRepository.GetValueByKeyAsync<bool>(
                GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

            var sendMessage = $"Вы успешно оформили заказ: \"{createPaymentOrderAggregateInput.FareRuleName}\"";

            await mailingsService.SendNotificationCreatedOrderAsync(
                createPaymentOrderAggregateInput.Account, sendMessage, isEnabledEmailNotifications,
                createPaymentOrderAggregateInput.Month).ConfigureAwait(false);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.
    
    /// <summary>
    /// Метод парсит результат для сохранения заказа в БД.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="createOrderRequest">Модель запроса.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="responseCheckStatusOrder">Статус ответа из ПС.</param>
    /// <param name="orderType">Тип заказа.</param>
    /// <param name="fareRuleName">Название заказа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <returns>Результирующая модель для сохранения в БД.</returns>
    private static async Task<CreatePaymentOrderInput> CreatePaymentOrderAsync(string paymentId,
        ICreateOrderInput createOrderInput, long userId, string responseCheckStatusOrder, OrderTypeEnum orderType,
        string? fareRuleName, short? month)
    {
        var createOrder = JsonConvert.DeserializeObject<PaymentStatusOutput>(responseCheckStatusOrder);
        var result = new CreatePaymentOrderInput
        {
            PaymentId = paymentId,
            Name = fareRuleName,
            UserId = userId,
            PaymentMonth = month,
            CurrencyType = CurrencyTypeEnum.RUB,
            Created = DateTime.Parse(createOrder!.Created),
            PaymentStatusSysName = createOrder.OrderStatus,
            PaymentStatusName = PaymentStatusEnum.Pending.GetEnumDescription(),
            OrderType = orderType
        };
        
        if (createOrderInput is CreateOrderPayMasterInput orderPayMasterInput)
        {
            result.Description = orderPayMasterInput.CreateOrderRequest.Invoice.Description;
            result.Price = orderPayMasterInput.CreateOrderRequest.Amount.Value;
        }

        else if (createOrderInput is CreateOrderYandexKassaInput orderYandexKassaInput)
        {
            result.Description = orderYandexKassaInput.CreateOrderRequest.Description;
            result.Price = orderYandexKassaInput.CreateOrderRequest.Amount.Value;
        }

        return await Task.FromResult(result);
    }

    #endregion
}