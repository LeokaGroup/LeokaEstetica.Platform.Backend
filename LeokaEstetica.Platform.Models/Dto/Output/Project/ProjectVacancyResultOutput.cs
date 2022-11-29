using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели списка вакансий проекта.
/// </summary>
public class ProjectVacancyResultOutput : IFrontError
{
    /// <summary>
    /// Список вакансий проекта.
    /// </summary>
    public IEnumerable<ProjectVacancyOutput> ProjectVacancies { get; set; }

    /// <summary>
    /// Кол-во вакансий проекта.
    /// </summary>
    public int Total => ProjectVacancies.Count();
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}