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

    /// <summary>
    /// Метод создает замечания проекта.
    /// </summary>
    /// <param name="createProjectRemarkInput">Список замечаний.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateProjectRemarksAsync(IEnumerable<ProjectRemarkEntity> projectRemarks);

    /// <summary>
    /// Метод получает замечания проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний.</returns>
    Task<List<ProjectRemarkEntity>> GetProjectRemarksAsync(long projectId);

    /// <summary>
    /// Метод получает замечания проекта, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="fields">Список названий полей..</param>
    /// <returns>Список замечаний.</returns>
    Task<List<ProjectRemarkEntity>> GetExistsProjectRemarksAsync(long projectId, IEnumerable<string> fields);

    /// <summary>
    /// Метод обновляет замечания проекта.
    /// </summary>
    /// <param name="projectRemarks">Список замечаний для обновления.</param>
    Task UpdateProjectRemarksAsync(List<ProjectRemarkEntity> projectRemarks);

    /// <summary>
    /// Метод отправляет замечания проекта владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// </summary>
    Task SendProjectRemarksAsync(long projectId, long userId);

    /// <summary>
    /// Метод проверяет, были ли внесены замечания проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак внесения замечаний.</returns>
    Task<bool> CheckExistsProjectRemarksAsync(long projectId);

    /// <summary>
    /// Метод возвращает название проекта по его Id.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns></returns>
    Task<string> GetProjectNameAsync(long projectId);

    /// <summary>
    /// Метод получает список комментариев проекта для модерации.
    /// </summary>
    /// <returns>Список комментариев.</returns>
    Task<IEnumerable<ProjectCommentModerationEntity>> GetProjectCommentsModerationAsync(long projectId);
}