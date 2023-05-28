using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Messaging.Abstractions.Project;

/// <summary>
/// Абстракция сервиса комментариев к проектам.
/// </summary>
public interface IProjectCommentsService
{
    /// <summary>
    /// Метод создает комментарий к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Текст комментария.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    Task CreateProjectCommentAsync(long projectId, string comment, string account, string token);

    /// <summary>
    /// Метод получает список комментариев проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    Task<IEnumerable<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId);
}