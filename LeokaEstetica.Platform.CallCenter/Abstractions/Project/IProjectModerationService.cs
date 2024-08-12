using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.CallCenter.Abstractions.Project;

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
    Task<ProjectOutput> GetProjectModerationByProjectIdAsync(long projectId);

    /// <summary>
    /// Метод одобряет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель модерации.</returns>
    Task<ApproveProjectOutput> ApproveProjectAsync(long projectId, string account);
    
    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель модерации.</returns>
    Task<RejectProjectOutput> RejectProjectAsync(long projectId, string account);

    /// <summary>
    /// Метод создает замечания проекта. 
    /// </summary>
    /// <param name="createProjectRemarkInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список замечаний проекта.</returns>
    Task<IEnumerable<ProjectRemarkEntity>> CreateProjectRemarksAsync(
        CreateProjectRemarkInput createProjectRemarkInput, string account);

    /// <summary>
    /// Метод отправляет замечания проекта владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// </summary>
    Task SendProjectRemarksAsync(long projectId, string account);
    
    /// <summary>
    /// Метод получает список замечаний проекта (не отправленные), если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний проекта.</returns>
    Task<IEnumerable<ProjectRemarkEntity>> GetProjectUnShippedRemarksAsync(long projectId);

    /// <summary>
    /// Метод получает комментарий проекта для просмотра.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Данные комментария.</returns>
    Task<ProjectCommentModerationEntity> GetCommentModerationByCommentIdAsync(long commentId);
    
    /// <summary>
    /// Метод одобряет комментарий проекта.
    /// </summary>
    /// <param name="commentId">Id комментарии.</param>
    /// <returns>Признак успешного подверждения.</returns>
    Task<bool> ApproveProjectCommentAsync(long commentId);
    
    /// <summary>
    /// Метод отклоняет комментарий проекта.
    /// </summary>
    /// <param name="commentId">Id комментарии.</param>
    /// <returns>Признак успешного подверждения.</returns>
    Task<ManagingProjectCommentModerationOutput> RejectProjectCommentAsync(long commentId);
}