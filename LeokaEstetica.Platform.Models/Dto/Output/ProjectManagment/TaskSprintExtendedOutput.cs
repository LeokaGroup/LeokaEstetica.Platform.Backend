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
    public string? DateStart { get; set; }

    /// <summary>
    /// Дата окончания спринта.
    /// </summary>
    public string? DateEnd { get; set; }

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
    /// Задачи спринта, если они есть.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskOutput>? SprintTasks { get; set; }
    
    /// <summary>
    /// Id исполнителя спринта (кто ответственный за спринт).
    /// </summary>
    public long? ExecutorId { get; set; }

    /// <summary>
    /// Id наблюдателей спринта.
    /// </summary>
    public IEnumerable<long>? WatcherIds { get; set; }

    /// <summary>
    /// Дата создания спринта.
    /// </summary>
    public string CreatedAt { get; set; }

    /// <summary>
    /// Кто создал спринт.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата обновления спринта.
    /// </summary>
    public string? UpdatedAt { get; set; }
    
    /// <summary>
    /// Кто обновил спринт.
    /// </summary>
    public long? UpdatedBy { get; set; }
    
    /// <summary>
    /// TODO: В будущем будет изменен на объект, содержащий фото и тд.
    /// ФИО наблюдателей задачи.
    /// </summary>
    public List<string>? WatcherNames { get; set; }

    /// <summary>
    /// Название исполнителя (email или логин).
    /// </summary>
    public string? ExecutorName { get; set; }

    /// <summary>
    /// Название автора задачи (email или логин).
    /// </summary>
    public string? AuthorName { get; set; }
}