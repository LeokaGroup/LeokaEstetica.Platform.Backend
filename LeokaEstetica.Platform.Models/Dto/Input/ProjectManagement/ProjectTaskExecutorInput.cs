namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели исполнителя задачи.
/// </summary>
public class ProjectTaskExecutorInput
{
    /// <summary>
    /// Id нового исполнителя.
    /// </summary>
    public int ExecutorId { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}