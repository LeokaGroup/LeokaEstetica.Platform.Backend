using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей сообщений диалогов Communications.DialogMessages.
/// </summary>
public class DialogMessageEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// FK на диалоги.
    /// </summary>
    public MainInfoDialogEntity Dialog { get; set; }

    /// <summary>
    /// Текст сообщения.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// FK на пользователей.
    /// </summary>
    public UserEntity User { get; set; }

    /// <summary>
    /// Признак принадлежности сообщения текущему пользователю.
    /// </summary>
    public bool IsMyMessage { get; set; }
}