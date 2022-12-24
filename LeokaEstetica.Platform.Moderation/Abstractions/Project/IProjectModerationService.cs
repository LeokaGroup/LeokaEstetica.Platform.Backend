using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output;

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
    Task<ProjectsModerationResult> ProjectsModerationAsync();

    /// <summary>
    /// Метод получает проект для просмотра/изменения.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    Task<UserProjectEntity> GetProjectModerationByProjectIdAsync(long projectId);

    /// <summary>
    /// Метод одобряет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Выходная модель модерации.</returns>
    Task<ApproveProjectOutput> ApproveProjectAsync(long projectId);
    
    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Выходная модель модерации.</returns>
    Task<ApproveProjectOutput> RejectProjectAsync(long projectId);
}