using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей вакансий проектов Projects.ProjectVacancies.
/// </summary>
public class ProjectVacancyEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ProjectVacancyId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long? VacancyId { get; set; }

    /// <summary>
    /// FK на вакансии пользователя.
    /// </summary>
    public UserVacancyEntity UserVacancy { get; set; }
}