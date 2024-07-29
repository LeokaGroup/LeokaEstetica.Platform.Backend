namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели включения задачи в эпик.
/// </summary>
public class IncludeTaskEpicInput
{
    /// <summary>
    /// Id эпика.
    /// </summary>
    public string? EpicId { get; set; }

    /// <summary>
    /// Id задач в рамках проекта, которые добавляются в эпик.
    /// </summary>
    public IEnumerable<string>? ProjectTaskIds { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}