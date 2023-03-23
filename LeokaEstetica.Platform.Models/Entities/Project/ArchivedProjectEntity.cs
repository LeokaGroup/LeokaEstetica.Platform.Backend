namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей архива проектов Projects.ArchivedProjects.
/// </summary>
public class ArchivedProjectEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ArchiveId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Дата архива.
    /// </summary>
    public DateTime DateArchived { get; set; }

    /// <summary>
    /// FK на проекты пользователя.
    /// </summary>
    public UserProjectEntity UserProject { get; set; }
}
