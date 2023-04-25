namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Moderation;

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
    public int Total { get; set; }
}