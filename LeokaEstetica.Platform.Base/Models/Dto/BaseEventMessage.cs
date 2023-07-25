using LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;

namespace LeokaEstetica.Platform.Base.Models.Dto;

/// <summary>
/// Базовый класс событий сообщений.
/// </summary>
public abstract class BaseEventMessage : IIntegrationEvent
{
    /// <summary>
    /// Дата публикации.
    /// </summary>
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
}