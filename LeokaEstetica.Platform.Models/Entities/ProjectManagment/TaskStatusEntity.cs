namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей статусов задач.
/// </summary>
public class TaskStatusEntity
{
    public TaskStatusEntity()
    {
        ProjectTasks = new HashSet<ProjectTaskEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// FK на задачу.
    /// </summary>
    public IEnumerable<ProjectTaskEntity> ProjectTasks { get; set; }
}