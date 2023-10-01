namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей статусов отклика на проект Projects.ProjectResponseStatuses.
/// </summary>
public class ProjectResponseStatuseEntity
{
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
}