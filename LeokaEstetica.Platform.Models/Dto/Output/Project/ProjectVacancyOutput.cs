using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели 
/// </summary>
public class ProjectVacancyOutput : IFrontError
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
    /// <summary>
    /// Список вакансий проекта.
    /// </summary>
    // public IEnumerable<UserVacancyOutput> ProjectVacancies { get; set; }
    //
    // /// <summary>
    // /// Крол-во вакансий проекта.
    // /// </summary>
    // public int Total => ProjectVacancies.Count();

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}