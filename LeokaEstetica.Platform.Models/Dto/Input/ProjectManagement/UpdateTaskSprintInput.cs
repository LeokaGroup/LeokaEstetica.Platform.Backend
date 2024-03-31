namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели обновления спринта задачи.
/// </summary>
public class UpdateTaskSprintInput
{
    /// <summary>
    /// Id спринта.
    /// </summary>
    public long SprintId { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public string ProjectTaskId { get; set; }
}