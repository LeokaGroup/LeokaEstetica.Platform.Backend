using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;
using LeokaEstetica.Platform.Models.Dto.Proxy.ProjectManagement;

namespace LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;

/// <summary>
/// Абстракция работы с кроликом.
/// </summary>
public interface IRabbitMqService
{
    /// <summary>
    /// Метод отправляет сообщение в очередь.
    /// </summary>
    /// <param name="event">Событие.</param>
    /// <param name="queueType">Тип очереди.</param>
    /// <param name="rabbitMqConfig">Конфиг с настройками RabbitMQ.</param>
    /// <param name="configEnv">Конфиг со средой окружения.</param>
    Task PublishAsync(IIntegrationEvent @event, string queueType, ProxyConfigRabbitMqOutput rabbitMqConfig,
        ProxyConfigEnvironmentOutput configEnv);
}