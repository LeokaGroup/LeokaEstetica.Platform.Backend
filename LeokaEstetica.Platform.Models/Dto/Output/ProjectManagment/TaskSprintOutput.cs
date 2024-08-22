namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели спринта, в который входит задача.
/// </summary>
public class TaskSprintOutput
{
    /// <summary>
    /// Id спринта.
    /// </summary>
    public long SprintId { get; set; }

    /// <summary>
    /// Id спринта в проекте.
    /// </summary>
    public long ProjectSprintId { get; set; }

    /// <summary>
    /// Название спринта.
    /// </summary>
    public string? SprintName { get; set; }
    
    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ProjectSprintId);
    
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string? TaskIdPrefix { get; set; }
}