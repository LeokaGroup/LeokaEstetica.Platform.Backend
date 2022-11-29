using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели вакансий проекта.
/// </summary>
public class ProjectVacancyOutput
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
    public long VacancyId { get; set; }

    /// <summary>
    /// FK на вакансии пользователя.
    /// </summary>
    public UserVacancyOutput UserVacancy { get; set; }
}