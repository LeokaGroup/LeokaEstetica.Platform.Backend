using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели 
/// </summary>
public class ProjectVacancyOutput : IFrontError
{
    /// <summary>
    /// Список вакансий проекта.
    /// </summary>
    public IEnumerable<UserVacancyOutput> ProjectVacancies { get; set; } = Enumerable.Empty<UserVacancyOutput>();

    /// <summary>
    /// Крол-во вакансий проекта.
    /// </summary>
    public int Total => ProjectVacancies.Count();

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}