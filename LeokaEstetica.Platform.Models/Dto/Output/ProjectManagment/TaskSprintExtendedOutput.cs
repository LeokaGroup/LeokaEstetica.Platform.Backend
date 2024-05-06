using LeokaEstetica.Platform.Models.Dto.Output.Template;

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
    public string? SprintStatusName { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id спринта в рамках проекта.
    /// </summary>
    public long ProjectSprintId { get; set; }

    /// <summary>
    /// Задачи спринта, если они есть.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskOutput>? SprintTasks { get; set; }
    
    /// <summary>
    /// Id исполнителя спринта, если они были заданы.
    /// </summary>
    public IEnumerable<long>? WatcherIds { get; set; }

    /// <summary>
    /// Id исполнителя спринта, если он был задан.
    /// </summary>
    public long? ExecutorId { get; set; }

    /// <summary>
    /// Id автора спринта.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания спринта.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления спринта.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Id пользователя, который обновил спринт.
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// Список названий наблюдателей.
    /// </summary>
    public List<string>? WatcherNames { get; set; }
}