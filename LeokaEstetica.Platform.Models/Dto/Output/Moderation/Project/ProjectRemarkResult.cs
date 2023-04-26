namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс результата списка замечаний проекта.
/// </summary>
public class ProjectRemarkResult
{
    /// <summary>
    /// Список замечаний проекта.
    /// </summary>
    public IEnumerable<ProjectRemarkOutput> ProjectRemark { get; set; }

    /// <summary>
    /// Кол-во замечаний всего.
    /// </summary>
    public int Total => ProjectRemark.Count();
}