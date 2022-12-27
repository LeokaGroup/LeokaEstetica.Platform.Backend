using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.Models.Entities.ProjectTeam;

/// <summary>
/// Класс сопоставляется с таблицей команды проекта Teams.ProjectsTeams.
/// </summary>
public class ProjectTeamEntity
{
    public ProjectTeamEntity()
    {
        ProjectTeamVacancies = new HashSet<ProjectTeamVacancyEntity>();
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
    /// Список вакансий в командах проектов.
    /// </summary>
    public ICollection<ProjectTeamVacancyEntity> ProjectTeamVacancies { get; set; }
}