namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели наблюдателей задачи.
/// </summary>
public class ProjectTaskWatcherInput
{
    /// <summary>
    /// Id нового наблюдателя задачи.
    /// </summary>
    public int WatcherId { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public string ProjectTaskId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}