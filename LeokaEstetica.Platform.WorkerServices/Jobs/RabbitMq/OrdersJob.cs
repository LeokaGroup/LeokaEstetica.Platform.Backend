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
using LeokaEstetica.Platform.Messaging.Factors;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.WorkerServices.Jobs.RabbitMq;

/// <summary>
/// Класс джобы консьюмера заказов кролика.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class OrdersJob : IJob
{
    private readonly IModel _channel;
    private readonly IPayMasterService _payMasterService;
    private readonly HttpClient _httpClient;
    private readonly ICommerceRepository _commerceRepository;
    private readonly ILogger<OrdersJob> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IGlobalConfigRepository _globalConfigRepository;

    /// <summary>
    /// Название очереди.
    /// </summary>
    private readonly string _queueName = string.Empty;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Зависимость конфигурации приложения.</param>
    /// <param name="payMasterService">Сервис ПС PayMaster.</param>
    /// <param name="httpClient">HttpClient.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="subscriptionService">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Репозиторий тарифов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    public OrdersJob(IConfiguration configuration, 
        IPayMasterService payMasterService,
        ICommerceRepository commerceRepository, 
        ILogger<OrdersJob> logger, 
        ISubscriptionService subscriptionService, 
        IFareRuleRepository fareRuleRepository,
        IGlobalConfigRepository globalConfigRepository)
    {
        _payMasterService = payMasterService;
        _httpClient = new HttpClient();
        _commerceRepository = commerceRepository;
        _logger = logger;
        _subscriptionService = subscriptionService;
        _fareRuleRepository = fareRuleRepository;
        _globalConfigRepository = globalConfigRepository;

        var factory = CreateRabbitMqConnectionFactory.CreateRabbitMqConnection(configuration);
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        
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

                // Проверяем статус платежа в ПС.
                // TODO: Обработать здесь NRE!
                var paymentId = orderEvent.PaymentId;
                var newOrderStatus = await _payMasterService.CheckOrderStatusAsync(paymentId, _httpClient);
                
                // Получаем старый статус платежа до проверки в ПС.
                var oldStatusSysName = PaymentStatus.GetPaymentStatusBySysName(orderEvent.StatusSysName);

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
                    }
                
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Ошибка при чтении очереди заказов.");
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
}