using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Communications.Models;

/// <summary>
/// Класс dto сообщений хаба.
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Код пользователя.
    /// </summary>
    public Guid UserCode { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Id пользователя, который создал сообщение.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Признак сообщения текущего пользователя.
    /// </summary>
    public bool IsMyMessage { get; set; }
    
    /// <summary>
    /// Модуль.
    /// </summary>
    public UserConnectionModuleEnum Module { get; set; }
}