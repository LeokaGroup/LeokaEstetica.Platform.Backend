namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели спринтов, у которых закончился срок.
/// </summary>
public class SprintEndDateOutput
{
    /// <summary>
    /// Id спринта - PK.
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