using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace LeokaEstetica.Platform.Messaging.Factors;

/// <summary>
/// Класс факторки для создания подключения к кролику.
/// </summary>
public static class CreateRabbitMqConnectionFactory
{
    /// <summary>
    /// Метод создает подключение к кролику со всеми параметрами подключения.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Объект с данными подключения к кролику.</returns>
    public static ConnectionFactory CreateRabbitMqConnection(IConfiguration configuration)
    {
        return new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            Password = configuration["RabbitMq:Password"],
            UserName = configuration["RabbitMq:UserName"],
            DispatchConsumersAsync = true,
            Port = AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = "/",
            ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
        };
    }
}