namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели диалога сообщений группы объекта абстрактной области чата.
/// Например, проекта.
/// </summary>
public class GroupObjectDialogMessageOutput
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
    public string? CreatedAt { get; set; }

    /// <summary>
    /// Id пользователя, который создал сообщение.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Название диалога.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// Id объекта.
    /// </summary>
    public long ObjectId { get; set; }
}