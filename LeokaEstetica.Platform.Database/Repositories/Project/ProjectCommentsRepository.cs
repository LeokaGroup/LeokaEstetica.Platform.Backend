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
public sealed class ProjectCommentsRepository : IProjectCommentsRepository
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
                Created = DateTime.Now,
                ProjectId = projectId,
                UserId = userId,
                IsMyComment = true
            };
            await _pgContext.ProjectComments.AddAsync(prj);
            await _pgContext.SaveChangesAsync();
            
            // Отправляем комментарий к проекту на модерацию.
            await _pgContext.ProjectCommentsModeration.AddAsync(new ProjectCommentModerationEntity
            {
                CommentId = prj.CommentId,
                DateModeration = DateTime.Now,
                StatusId = (int)ProjectModerationStatusEnum.ModerationProject
            });
            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        
        catch
        {
            await transaction.RollbackAsync();
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
                where pc.ProjectId == projectId
                join pcm in _pgContext.ProjectCommentsModeration
                    on pc.CommentId
                    equals pcm.CommentId
                where !new[]
                    {
                        (int)ProjectModerationStatusEnum.ModerationProject, // На модерации.
                        (int)ProjectModerationStatusEnum.RejectedProject // Отклонен.
                    }
                    .Contains(pcm.StatusId)
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
}