using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace LeokaEstetica.Platform.Messaging.Factors;

/// <summary>
/// Класс факторки, которая создает подключение как синглтон.
/// </summary>
public static class CreateRabbitMqConnectionSingletonFactory
{
    private static IConnection _connection;

    /// <summary>
    /// Метод создает подключение к кролику со всеми параметрами подключения.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Объект с данными подключения к кролику.</returns>
    public static IConnection CreateRabbitMqConnection(IConfiguration configuration)
    {
        if (_connection == null)
        {
            var connection = new ConnectionFactory
            {
                HostName = configuration["RabbitMq:HostName"],
                Password = configuration["RabbitMq:Password"],
                UserName = configuration["RabbitMq:UserName"],
                DispatchConsumersAsync = true,
                Port = AmqpTcpEndpoint.UseDefaultPort,
                VirtualHost = "/",
                ContinuationTimeout = new TimeSpan(0, 0, 10, 0)
            };
            
            _connection = connection.CreateConnection();
        }

        return _connection;
    }
}