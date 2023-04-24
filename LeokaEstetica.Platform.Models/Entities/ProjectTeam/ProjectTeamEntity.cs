using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.Models.Entities.ProjectTeam;

/// <summary>
/// Класс сопоставляется с таблицей команды проекта Teams.ProjectsTeams.
/// </summary>
public class ProjectTeamEntity
{
    public ProjectTeamEntity()
    {
        ProjectTeamMembers = new HashSet<ProjectTeamMemberEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Дата создания команды.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public CatalogProjectEntity CatalogProject { get; set; }

    /// <summary>
    /// Список участников команд проектов.
    /// </summary>
    public ICollection<ProjectTeamMemberEntity> ProjectTeamMembers { get; set; }
}