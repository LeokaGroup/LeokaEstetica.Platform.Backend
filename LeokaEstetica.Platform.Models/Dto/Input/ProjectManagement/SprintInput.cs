namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Базовый класс входной модели спринта.
/// </summary>
public class SprintInput
{
    /// <summary>
    /// Id спринта.
    /// </summary>
    public long SprintId { get; set; }

    /// <summary>
    /// Id спринта в рамках проекта.
    /// </summary>
    public long ProjectSprintId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}