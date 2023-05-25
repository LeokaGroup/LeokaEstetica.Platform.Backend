using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
using LeokaEstetica.Platform.Messaging.Consts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace LeokaEstetica.Platform.Messaging.Services.RabbitMq;

/// <summary>
/// Класс реализует методы сервиса кролика.
/// </summary>
public class RabbitMqService : IRabbitMqService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Зависимость конфигурации приложения.</param>
    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Метод отправляет сообщение в очередь.
    /// </summary>
    /// <param name="event">Событие.</param>
    /// <param name="queueType">Тип очереди.</param>
    public Task PublishAsync(IIntegrationEvent @event, QueueTypeEnum queueType)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:LocalHostName"],
            DispatchConsumersAsync = true
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var queueName = queueType.GetEnumDescription();

        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: ExchangeTypeConst.Direct, routingKey: queueName, basicProperties: null,
            body: body);
        
        return Task.CompletedTask;
    }
}