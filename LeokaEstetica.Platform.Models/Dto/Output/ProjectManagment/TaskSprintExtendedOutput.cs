namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Выходная модель спринта - расширенная.
/// </summary>
public class TaskSprintExtendedOutput : TaskSprintOutput
{
    /// <summary>
    /// Дата начала спринта.
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Дата окончания спринта.
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Описание спринта.
    /// </summary>
    public string? SprintGoal { get; set; }

    /// <summary>
    /// Id статуса спринта.
    /// </summary>
    public int SprintStatusId { get; set; }
    
    /// <summary>
    /// Название статуса спринта.
    /// </summary>
    public int SprintStatusName { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id спринта в рамках проекта.
    /// </summary>
    public long ProjectSprintId { get; set; }
}