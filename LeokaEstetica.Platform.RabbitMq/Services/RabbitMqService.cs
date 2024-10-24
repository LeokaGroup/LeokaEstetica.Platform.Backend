using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Notifications")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.RabbitMq.Services;

/// <summary>
/// Класс реализует методы сервиса кролика.
/// </summary>
internal sealed class RabbitMqService : IRabbitMqService
{
    private IModel? _channel;

    /// <summary>
    /// Счетчик кол-ва подключений во избежание дублей подключений.
    /// </summary>
    private static uint _counter;

    #region Публичные методы.

    /// <inheritdoc />
    public async Task PublishAsync(IIntegrationEvent @event, string queueType, IConfiguration configuration,
        QueueTypeEnum flags)
    {
        var connection = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            Password = configuration["RabbitMq:Password"],
            UserName = configuration["RabbitMq:UserName"],
            DispatchConsumersAsync = true,
            Port = AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = configuration["RabbitMq:VirtualHost"],
            ContinuationTimeout = new TimeSpan(0, 0, 10, 0)
        };

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counter < 1)
        {
            await CreateQueueAsync(connection, configuration, flags);

            _counter++;
        }

        // TODO: Проверить, не будет ли плодить подключения это?
        if (_counter == 1 && _channel is null)
        {
            await CreateQueueAsync(connection, configuration, flags);
        }

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty, routingKey: queueType, basicProperties: null, body: body);

        await Task.CompletedTask;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает новую очередь.
    /// </summary>
    /// <param name="connection">Факторка подключения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="flags">Флаги очереди.</param>
    private async Task CreateQueueAsync(ConnectionFactory connection, IConfiguration configuration,
        QueueTypeEnum flags)
    {
        var connection1 = connection.CreateConnection();
        _channel = connection1.CreateModel();

        _channel.QueueDeclare(queue: string.Empty.CreateQueueDeclareNameFactory(configuration["Environment"],
            flags), durable: false, exclusive: false, autoDelete: false, arguments: null);

        await Task.CompletedTask;
    }

    #endregion
}