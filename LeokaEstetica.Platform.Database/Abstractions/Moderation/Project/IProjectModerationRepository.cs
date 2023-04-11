using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;

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
    /// <returns>Признак подиверждения проекта.</returns>
    Task<bool> ApproveProjectAsync(long projectId);
    
    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак отклонения проекта.</returns>
    Task<bool> RejectProjectAsync(long projectId);

    /// <summary>
    /// Метод получает название проекта по его Id.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Название проекта.</returns>
    Task<string> GetProjectNameByIdAsync(long projectId);
    
    /// <summary>
    /// Метод отправляет уведомление в приложении при одобрении проекта модератором.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="projectName">Название проекта.</param>
    Task AddNotificationApproveProjectAsync(long projectId, long userId, string projectName);
    
    /// <summary>
    /// Метод отправляет уведомление в приложении при отклонении проекта модератором.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="projectName">Название проекта.</param>
    Task AddNotificationRejectProjectAsync(long projectId, long userId, string projectName);
}