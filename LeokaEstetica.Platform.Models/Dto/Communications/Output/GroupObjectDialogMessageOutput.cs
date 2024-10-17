namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели сообщений диалога группы объекта абстрактной области чата.
/// </summary>
public class GroupObjectDialogMessageOutput
{
    /// <summary>
    /// Id сообщения.
    /// </summary>
    public long MessageId { get; set; }
    
    /// <summary>
    /// Id пользователя, который создал сообщение.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public string? CreatedAt { get; set; }

    /// <summary>
    /// Если текущий пользователь создал сообщение.
    /// </summary>
    public bool IsMyMessage { get; set; }
}