using LeokaEstetica.Platform.Base.Models.Dto;

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
    public ScrumMasterAiMessageEvent(string? message, string? token, long userId)
    {
        Message = message;
        Token = token;
        UserId = userId;
    }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Токен пользователя.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}