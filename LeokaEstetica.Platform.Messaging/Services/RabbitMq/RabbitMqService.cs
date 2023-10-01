using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace LeokaEstetica.Platform.Messaging.Services.RabbitMq;

/// <summary>
/// Класс реализует методы сервиса кролика.
/// </summary>
internal sealed class RabbitMqService : IRabbitMqService
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
    public Task PublishAsync(IIntegrationEvent @event, string queueType)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"],
            DispatchConsumersAsync = true
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueType, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: string.Empty, routingKey: queueType, basicProperties: null,
            body: body);
        
        return Task.CompletedTask;
    }
}