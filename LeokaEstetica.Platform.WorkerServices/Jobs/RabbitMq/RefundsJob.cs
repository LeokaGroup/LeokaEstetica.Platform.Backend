using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Refunds;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Messaging.Factors;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Enums;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.WorkerServices.Jobs.RabbitMq;

/// <summary>
/// Класс джобы консьюмера возвратов кролика.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class RefundsJob : IJob
{
    private readonly IModel _channel;
    private readonly IPayMasterService _payMasterService;
    private readonly HttpClient _httpClient;
    private readonly ICommerceRepository _commerceRepository;
    private readonly ILogger<OrdersJob> _logger;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IPachcaService _pachcaService;
    
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
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    public RefundsJob(IConfiguration configuration, 
        IPayMasterService payMasterService,
        ICommerceRepository commerceRepository, 
        ILogger<OrdersJob> logger,
        IGlobalConfigRepository globalConfigRepository,
        IPachcaService pachcaService)
    {
        _payMasterService = payMasterService;
        _httpClient = new HttpClient();
        _commerceRepository = commerceRepository;
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _pachcaService = pachcaService;

        var factory = CreateRabbitMqConnectionFactory.CreateRabbitMqConnection(configuration);
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        
        var flags = QueueTypeEnum.RefundsQueue | QueueTypeEnum.RefundsQueue;

        _channel.QueueDeclare(queue: _queueName.CreateQueueDeclareNameFactory(configuration, flags), durable: false,
            exclusive: false, autoDelete: false, arguments: null);
    }

    /// <summary>
    /// Метод запускает логику фоновой задачи.
    /// </summary>
    /// <param name="context">Выполняемый контекст джобы.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.REFUNDS_JOB_MODE_ENABLED);

        if (!isEnabledJob)
        {
            return;
        }

        await CheckRefundStatusAsync();
        
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Метод выполняет работу джобы.
    /// </summary>
    private async Task CheckRefundStatusAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            _logger.LogInformation("Начали обработку сообщения из очереди возвратов...");
            
            // Если очередь не пуста, то будем парсить результат для проверки статуса возвратов.
            if (!ea.Body.IsEmpty)
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var refundEvent = JsonConvert.DeserializeObject<RefundEvent>(message);
                
                if (refundEvent is null)
                {
                    throw new InvalidOperationException("Событие не содержит нужных данных." +
                                                        $" Получили сообщение из очереди возвратов: {message}");
                }

                // Проверяем статус возврата в ПС.
                var paymentId = refundEvent.PaymentId;
                var newRefundStatus = await _payMasterService.CheckRefundStatusAsync(paymentId, _httpClient);
                
                // Получаем старый статус возврата до проверки в ПС.
                var oldStatusSysName = RefundStatus.GetPaymentStatusBySysName(refundEvent.Status);

                // Если статус возврата изменился в ПС, то обновляем его статус в БД.
                if (newRefundStatus != oldStatusSysName)
                {
                    try
                    {
                        var refundId = refundEvent.RefundId;
                        var refundOrderId = refundEvent.RefundOrderId;
                        var updatedOrderStatus = await _commerceRepository.UpdateRefundStatusAsync(
                            newRefundStatus.GetEnumDescription(), paymentId, refundId, refundOrderId);

                        if (!updatedOrderStatus)
                        {
                            var ex = new InvalidOperationException("Не удалось найти возврат для обновления статуса. " +
                                                                   $"RefundId: {refundId}." +
                                                                   $"PaymentId: {paymentId}" +
                                                                   $"RefundOrderId: {refundOrderId}");
                            throw ex;
                        }
                    }
                
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Ошибка при чтении очереди возвратов.");
                        
                        await _pachcaService.SendNotificationErrorAsync(ex);
                        
                        throw;
                    }
                    
                    // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                    _channel.BasicAck(ea.DeliveryTag, false);
                }

                // Статус в ПС не изменился, оставляем его в очереди.
                else
                {
                    _logger.LogInformation("Оставили сообщение в очереди возвратов...");
                    _channel.BasicRecoverAsync(false);
                }
            }
            
            _logger.LogInformation("Закончили обработку сообщения из очереди возвратов...");

            await Task.Yield();
        };

        _channel.BasicConsume(_queueName, false, consumer);

        await Task.CompletedTask;
    }
}