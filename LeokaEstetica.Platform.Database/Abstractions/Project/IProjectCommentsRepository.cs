using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Database.Abstractions.Project;

/// <summary>
/// Абстракция репозитория комментариев проектов.
/// </summary>
public interface IProjectCommentsRepository
{
    /// <summary>
    /// Метод создает комментарий к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Текст комментария.</param>
    /// <param name="userId">Id пользователя.</param>
    Task CreateProjectCommentAsync(long projectId, string comment, long userId);
    
    /// <summary>
    /// Метод получает список комментариев проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    Task<IEnumerable<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId);
    
    /// <summary>
    /// Метод получает все комментарии проектов.
    /// В память все это не выгружаем.
    /// </summary>
    /// <returns>Запрос списка комментариев проектов.</returns>
    Task<IQueryable<ProjectCommentEntity>> GetAllProjectCommentsAsync();
}