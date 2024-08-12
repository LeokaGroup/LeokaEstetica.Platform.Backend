namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс результата списка замечаний проекта.
/// </summary>
public class ProjectRemarkResult
{
    /// <summary>
    /// Список замечаний проекта.
    /// </summary>
    public IEnumerable<ProjectRemarkOutput>? ProjectRemarks { get; set; }

    /// <summary>
    /// Кол-во замечаний всего.
    /// </summary>
    public int Total => ProjectRemarks?.Count() ?? 0;
}