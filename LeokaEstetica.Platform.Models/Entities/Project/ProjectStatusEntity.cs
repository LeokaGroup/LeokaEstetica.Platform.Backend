namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей статусов проектов Projects.ProjectStatuses.
/// </summary>
public class ProjectStatusEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string ProjectStatusSysName { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string ProjectStatusName { get; set; }
}