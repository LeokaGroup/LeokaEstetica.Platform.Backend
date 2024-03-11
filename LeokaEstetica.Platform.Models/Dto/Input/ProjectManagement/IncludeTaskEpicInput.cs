namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели включения задачи в эпик.
/// </summary>
public class IncludeTaskEpicInput
{
    /// <summary>
    /// Id эпика.
    /// </summary>
    public long EpicId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id проекта в рамках задачи.
    /// </summary>
    public string ProjectTaskId { get; set; }
}