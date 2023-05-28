namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс выходной модели списка проектов на модерации.
/// </summary>
public class ProjectsModerationResult
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    public IEnumerable<ProjectModerationOutput> Projects { get; set; }

    /// <summary>
    /// Всего проектов на модерации.
    /// </summary>
    public int Total => Projects.Count();
}