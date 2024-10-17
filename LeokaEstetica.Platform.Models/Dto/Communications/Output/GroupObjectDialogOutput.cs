namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс выходной модели диалога группы объекта абстрактной области чата.
/// Например, проекта.
/// </summary>
public class GroupObjectDialogOutput
{
    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Последнее сообщение диалога.
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public string? CreatedAt { get; set; }

    /// <summary>
    /// Название диалога.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// Id объекта.
    /// </summary>
    public long ObjectId { get; set; }

    /// <summary>
    /// Вложенные элементы.
    /// </summary>
    public List<GroupObjectDialogMessageOutput>? Items { get; set; }
}