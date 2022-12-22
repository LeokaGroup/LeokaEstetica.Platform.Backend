using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

namespace LeokaEstetica.Platform.Moderation.Abstractions.Project;

/// <summary>
/// Абстракция модерации проектов.
/// </summary>
public interface IProjectModerationService
{
    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    Task<IEnumerable<ProjectModerationOutput>> ProjectsModerationAsync();
}