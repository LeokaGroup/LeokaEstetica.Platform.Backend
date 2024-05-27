using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
using LeokaEstetica.Platform.Models.Dto.Proxy.ProjectManagement;
using Newtonsoft.Json;
using RabbitMQ.Client;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Notifications")]

namespace LeokaEstetica.Platform.Messaging.Services.RabbitMq;

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

    public RabbitMqService(IModel? channelMessages)
    {
        _channel = channelMessages;
    }

    /// <inheritdoc />
    public async Task PublishAsync(IIntegrationEvent @event, string queueType,
        ProxyConfigRabbitMqOutput rabbitMqConfig,
        ProxyConfigEnvironmentOutput configEnv)
    {
        var connection = new ConnectionFactory
        {
            HostName = rabbitMqConfig.HostName,
            Password = rabbitMqConfig.Password,
            UserName = rabbitMqConfig.UserName,
            DispatchConsumersAsync = true,
            Port = AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = rabbitMqConfig.VirtualHost,
            ContinuationTimeout = new TimeSpan(0, 0, 10, 0)
        };

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counter < 1)
        {
            var flags = QueueTypeEnum.ScrumMasterAiMessage | QueueTypeEnum.ScrumMasterAiMessage;

            var connection1 = connection.CreateConnection();
            _channel = connection1.CreateModel();

            _channel.QueueDeclare(queue: string.Empty.CreateQueueDeclareNameFactory(configEnv.Environment, flags),
                durable: false, exclusive: false, autoDelete: true, arguments: null);

            _counter++;
        }

        // var factory = CreateRabbitMqConnectionFactory.CreateRabbitMqConnection(_configuration);
        // using var connection1 = factory.CreateConnection();
        // using var channel = connection.CreateModel();
        //
        // channel.QueueDeclare(queue: queueType, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty, routingKey: queueType, basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }
}