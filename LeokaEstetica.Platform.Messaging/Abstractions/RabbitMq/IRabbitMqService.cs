using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;

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
    Task PublishAsync(IIntegrationEvent @event, string queueType);
}