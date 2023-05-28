namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс сообщения диалога.
/// </summary>
public class DialogMessageOutput
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
    /// Текст сообщения.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public string Created { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Признак принадлежности сообщения текущему пользователю.
    /// </summary>
    public bool IsMyMessage { get; set; }
}