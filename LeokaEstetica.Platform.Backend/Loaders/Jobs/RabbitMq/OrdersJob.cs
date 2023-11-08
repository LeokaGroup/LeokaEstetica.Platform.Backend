using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Messaging.Factors;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs.RabbitMq;

/// <summary>
/// Класс джобы консьюмера заказов кролика.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class OrdersJob : IJob, IDisposable
{
    private IModel _channel;
    private IConnection _connection;
    private readonly ICommerceRepository _commerceRepository;
    private readonly ILogger<OrdersJob> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IPachcaService _pachcaService;
    private readonly ICommerceService _commerceService;

    /// <summary>
    /// Название очереди.
    /// </summary>
    private readonly string _queueName = string.Empty;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Зависимость конфигурации приложения.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="subscriptionService">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Репозиторий тарифов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    public OrdersJob(IConfiguration configuration,
        ICommerceRepository commerceRepository, 
        ILogger<OrdersJob> logger, 
        ISubscriptionService subscriptionService, 
        IFareRuleRepository fareRuleRepository,
        IGlobalConfigRepository globalConfigRepository,
        IPachcaService pachcaService,
        ICommerceService commerceService)
    {
        _commerceRepository = commerceRepository;
        _logger = logger;
        _subscriptionService = subscriptionService;
        _fareRuleRepository = fareRuleRepository;
        _globalConfigRepository = globalConfigRepository;
        _pachcaService = pachcaService;
        _commerceService = commerceService;

        _connection = CreateRabbitMqConnectionSingletonFactory.CreateRabbitMqConnection(configuration);
        _channel = CreateRabbitMqChannelSingletonFactory.CreateRabbitMqChannel(_connection);

        var flags = QueueTypeEnum.OrdersQueue | QueueTypeEnum.OrdersQueue;
        _channel.QueueDeclare(queue: _queueName.CreateQueueDeclareNameFactory(configuration, flags),
            durable: false, exclusive: false, autoDelete: false, arguments: null);   
    }
    
    /// <summary>
    /// Метод запускает логику фоновой задачи.
    /// </summary>
    /// <param name="context">Выполняемый контекст джобы.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.ORDERS_JOB_MODE_ENABLED);

        if (!isEnabledJob)
        {
            return;
        }
        
        await CheckOrderStatusAsync();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод выполняет работу джобы.
    /// </summary>
    private async Task CheckOrderStatusAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            _logger.LogInformation("Начали обработку сообщения из очереди заказов...");

            // Если очередь не пуста, то будем парсить результат для проверки статуса заказов.
            if (!ea.Body.IsEmpty)
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var orderEvent = JsonConvert.DeserializeObject<OrderEvent>(message);

                if (orderEvent is null)
                {
                    throw new InvalidOperationException("Событие не содержит нужных данных." +
                                                        $" Получили сообщение из очереди заказов: {message}");
                }

                PaymentStatusEnum newOrderStatus;
                PaymentStatusEnum oldStatusSysName;
                string paymentId;

                try
                {
                    // Проверяем статус платежа в ПС.
                    paymentId = orderEvent.PaymentId;
                    newOrderStatus = await _commerceService.CheckOrderStatusAsync(paymentId);
                
                    // Получаем старый статус платежа до проверки в ПС.
                    oldStatusSysName = PaymentStatus.GetPaymentStatusBySysName(orderEvent.StatusSysName);
                }
                
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Ошибка при чтении очереди заказов.");
                        
                    await _pachcaService.SendNotificationErrorAsync(ex);
                    throw;
                }

                // Если статус заказа изменился в ПС, то обновляем его статус в БД.
                if (newOrderStatus != oldStatusSysName)
                {
                    try
                    {
                        var orderId = orderEvent.OrderId;
                        var updatedOrderStatus = await _commerceRepository.UpdateOrderStatusAsync(
                            newOrderStatus.ToString(), newOrderStatus.GetEnumDescription(), paymentId, orderId);

                        if (!updatedOrderStatus)
                        {
                            var ex = new InvalidOperationException("Не удалось найти заказ для обновления статуса. " +
                                                                   $"OrderId: {orderId}." +
                                                                   $"PaymentId: {paymentId}");
                            throw ex;
                        }

                        var publicId = orderEvent.PublicId;
                        var fareRule = await _fareRuleRepository.GetByPublicIdAsync(publicId);

                        // Для бесплатного тарифа нет срока подписки.
                        if (!fareRule.IsFree)
                        {
                            // Проставляем подписку и даты подписки пользователю.
                            await _subscriptionService.SetUserSubscriptionAsync(orderEvent.UserId, publicId,
                                orderEvent.Month, orderId, fareRule.RuleId);
                        }
                        
                        // Если статус платежа в ПС ожидает подтверждения, то подтверждаем его, чтобы списать ДС.
                        // Подтвердить в ПС можно только заказы в статусе waiting_for_capture.
                        if (newOrderStatus == PaymentStatusEnum.WaitingForCapture)
                        {
                            await _commerceService.ConfirmPaymentAsync(paymentId,
                                new Amount(orderEvent.Price, orderEvent.Currency));
                        }
                    }
                
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Ошибка при чтении очереди заказов.");
                        
                        await _pachcaService.SendNotificationErrorAsync(ex);
                        throw;
                    }
                    
                    // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                    _channel.BasicAck(ea.DeliveryTag, false);
                }

                // Статус в ПС не изменился, оставляем его в очереди.
                else
                {
                    _logger.LogInformation("Оставили сообщение в очереди заказов...");
                    _channel.BasicRecoverAsync(false);
                }
            }

            _logger.LogInformation("Закончили обработку сообщения из очереди заказов...");

            await Task.Yield();
        };

        _channel.BasicConsume(_queueName, false, consumer);

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();

        _channel = null;
        _connection = null;
    }
}