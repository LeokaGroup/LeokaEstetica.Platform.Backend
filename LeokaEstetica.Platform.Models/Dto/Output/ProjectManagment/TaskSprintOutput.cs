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
    /// Название спринта.
    /// </summary>
    public string? SprintName { get; set; }
}