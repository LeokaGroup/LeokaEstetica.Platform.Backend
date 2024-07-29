namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели типов ошибок, задач, историй.
/// </summary>
public class ProjectTaskTypeOutput
{
    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }
    
    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TaskTypeId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string? TaskStatusName { get; set; }
}