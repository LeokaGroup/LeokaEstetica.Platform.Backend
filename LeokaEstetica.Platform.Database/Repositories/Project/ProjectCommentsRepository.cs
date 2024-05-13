using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Project;

/// <summary>
/// Класс реализует методы репозитория комментариев к проектам.
/// </summary>
internal sealed class ProjectCommentsRepository : IProjectCommentsRepository
{
    private readonly PgContext _pgContext;

    public ProjectCommentsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод создает комментарий к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Текст комментария.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task CreateProjectCommentAsync(long projectId, string comment, long userId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            var prj = new ProjectCommentEntity
            {
                Comment = comment,
                Created = DateTime.UtcNow,
                ProjectId = projectId,
                UserId = userId,
                IsMyComment = true,
                ModerationStatusId = (int)ProjectCommentModerationEnum.ModerationComment
            };
            await _pgContext.ProjectComments.AddAsync(prj);
            await _pgContext.SaveChangesAsync();
            
            // Отправляем комментарий к проекту на модерацию.
            var prjModeration = new ProjectCommentModerationEntity
            {
                CommentId = prj.CommentId,
                DateModeration = DateTime.UtcNow,
                ModerationStatusId = (int)ProjectCommentModerationEnum.ModerationComment
            };
            await _pgContext.ProjectCommentsModeration.AddAsync(prjModeration);
            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод получает список комментариев проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    public async Task<IEnumerable<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId)
    {
        var result = await (from pc in _pgContext.ProjectComments
                join pcm in _pgContext.ProjectCommentsModeration
                    on pc.CommentId
                    equals pcm.CommentId
                where pc.ProjectId == projectId &&
                      !new[]
                          {
                              (int)ProjectCommentModerationEnum.ModerationComment, // На модерации.
                              (int)ProjectCommentModerationEnum.RejectedComment // Отклонен.
                          }
                          .Contains(pcm.ModerationStatusId)
                select new ProjectCommentEntity
                {
                    CommentId = pc.CommentId,
                    Comment = pc.Comment,
                    Created = pc.Created,
                    UserId = pc.UserId,
                    IsMyComment = pc.IsMyComment,
                    ProjectId = pc.ProjectId
                })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает все комментарии проектов.
    /// В память все это не выгружаем.
    /// </summary>
    /// <returns>Запрос списка комментариев проектов.</returns>
    public async Task<IQueryable<ProjectCommentEntity>> GetAllProjectCommentsAsync()
    {
        var result = (from pc in _pgContext.ProjectComments
                join pcm in _pgContext.ProjectCommentsModeration
                    on pc.CommentId
                    equals pcm.CommentId
                select new ProjectCommentEntity
                {
                    CommentId = pc.CommentId,
                    Comment = pc.Comment,
                    Created = pc.Created,
                    UserId = pc.UserId,
                    IsMyComment = pc.IsMyComment,
                    ProjectId = pc.ProjectId,
                    ModerationStatusId = pc.ModerationStatusId
                })
            .AsQueryable();

        if (_pgContext.ProjectCommentsModeration.AsQueryable().Any())
        {
            result = result.Where(pcm => !new[]
                {
                    (long)ProjectCommentModerationEnum.ModerationComment, // На модерации.
                    (long)ProjectCommentModerationEnum.RejectedComment // Отклонен.
                }
                .Contains(pcm.ModerationStatusId));
        }

        return await Task.FromResult(result);
    }
}