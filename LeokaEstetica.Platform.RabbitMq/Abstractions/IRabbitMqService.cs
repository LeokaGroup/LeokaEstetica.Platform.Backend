using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;
using LeokaEstetica.Platform.Base.Enums;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.RabbitMq.Abstractions;

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
    /// <param name="configuration">Конфиг приложения.</param>
    /// <param name="flags">Какие флаги присвоить очереди.</param>
    Task PublishAsync(IIntegrationEvent @event, string queueType, IConfiguration configuration, QueueTypeEnum flags);
}