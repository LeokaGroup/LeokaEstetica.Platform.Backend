using LeokaEstetica.Platform.Models.Entities.ProjectTeam;

namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей каталога вакансий Vacancies.CatalogVacancies.
/// </summary>
public class CatalogVacancyEntity
{
    public CatalogVacancyEntity()
    {
        ProjectTeamVacancies = new HashSet<ProjectTeamVacancyEntity>();
        ProjectTeamMembers = new HashSet<ProjectTeamMemberEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long CatalogVacancyId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserVacancyEntity Vacancy { get; set; }

    /// <summary>
    /// Список вакансий в команде проекта.
    /// </summary>
    public ICollection<ProjectTeamVacancyEntity> ProjectTeamVacancies { get; set; }

    /// <summary>
    /// Список участников команд проектов.
    /// </summary>
    public ICollection<ProjectTeamMemberEntity> ProjectTeamMembers { get; set; }
}