using LeokaEstetica.Platform.Base.Models.Dto;
using LeokaEstetica.Platform.Core.Enums;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.ScrumMasterAi;

/// <summary>
/// Класс события
/// </summary>
public class ScrumMasterAiMessageEvent : BaseEventMessage
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="scrumMasterAiEventType">Тип ивента нейросети.</param>
    public ScrumMasterAiMessageEvent(string? message, string? token, long userId,
        ScrumMasterAiEventTypeEnum scrumMasterAiEventType)
    {
        Message = message;
        Token = token;
        UserId = userId;
        ScrumMasterAiEventType = scrumMasterAiEventType;
    }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Токен пользователя.
    /// </summary>
    public string? Token { get; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; }

    /// <summary>
    /// Тип ивента нейросети.
    /// </summary>
    public ScrumMasterAiEventTypeEnum ScrumMasterAiEventType { get; }
}