using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Enums;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LeokaEstetica.Platform.WorkerServices.Jobs.RabbitMq;

/// <summary>
/// Класс джобы консьюмера заказов кролика.
/// </summary>
public class OrdersJob : BackgroundService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IPayMasterService _payMasterService;
    private readonly HttpClient _httpClient;
    private readonly ICommerceRepository _commerceRepository;
    private readonly ILogger<OrdersJob> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Зависимость конфигурации приложения.</param>
    /// <param name="payMasterService">Сервис ПС PayMaster.</param>
    /// <param name="httpClient">HttpClient.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="logger">Сервис логов.</param>
    public OrdersJob(IConfiguration configuration, 
        IPayMasterService payMasterService,
        ICommerceRepository commerceRepository, 
        ILogger<OrdersJob> logger)
    {
        _payMasterService = payMasterService;
        _httpClient = new HttpClient();
        _commerceRepository = commerceRepository;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            DispatchConsumersAsync = true
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: QueueTypeEnum.OrdersQueue.GetEnumDescription(), durable: false, exclusive: false,
            autoDelete: false, arguments: null);
    }
    
    /// <summary>
    /// Метод запускает логику фоновой задачи.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CheckOrderStatusAsync(stoppingToken);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод выполняет работу джобы.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены таски.</param>
    private async Task CheckOrderStatusAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

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
                            newOrderStatus.GetEnumDescription(), newOrderStatus.ToString(), paymentId, orderId);

                        if (!updatedOrderStatus)
                        {
                            var ex = new InvalidOperationException("Не удалось найти заказ для обновления статуса. " +
                                                                   $"OrderId: {orderId}." +
                                                                   $"PaymentId: {paymentId}");
                            throw ex;
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

        _channel.BasicConsume(QueueTypeEnum.OrdersQueue.GetEnumDescription(), false, consumer);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод очищает ресурсы.
    /// </summary>
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        _httpClient?.Dispose();
        base.Dispose();
    }
}