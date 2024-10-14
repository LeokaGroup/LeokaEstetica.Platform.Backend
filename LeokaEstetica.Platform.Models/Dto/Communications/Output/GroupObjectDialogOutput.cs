namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели диалога группы объекта абстрактной области чата.
/// Например, проекта.
/// </summary>
public class GroupObjectDialogOutput
{
    /// <summary>
    /// Id сообщения.
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Id пользователя, который создал сообщение.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Признак принадлежности сообщения текущему пользователю.
    /// </summary>
    public bool IsMyMessage { get; set; }
}