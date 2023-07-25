namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс списка замечаний проекта входной модели.
/// </summary>
public class CreateProjectRemarkInput
{
    /// <summary>
    /// Список замечаний.
    /// </summary>
    public IEnumerable<ProjectRemarkInput> ProjectRemarks { get; set; }
}