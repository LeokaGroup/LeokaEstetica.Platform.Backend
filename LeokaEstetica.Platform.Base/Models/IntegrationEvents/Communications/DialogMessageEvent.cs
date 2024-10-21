using LeokaEstetica.Platform.Base.Models.Dto;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.Communications;

/// <summary>
/// Класс события сообщений диалога чата.
/// </summary>
public class DialogMessageEvent : BaseEventMessage
{
    /// <summary>
    /// Текст сообщения.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Id пользователя, который создал сообщение.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Код пользователя.
    /// </summary>
    public Guid UserCode { get; set; }

    /// <summary>
    /// Модуль.
    /// </summary>
    public UserConnectionModuleEnum Module { get; set; }
}