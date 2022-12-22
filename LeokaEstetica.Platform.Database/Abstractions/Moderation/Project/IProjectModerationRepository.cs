using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;

/// <summary>
/// Абстракция репозитория модерации проектов.
/// </summary>
public interface IProjectModerationRepository
{
    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    Task<IEnumerable<ModerationProjectEntity>> ProjectsModerationAsync();
}