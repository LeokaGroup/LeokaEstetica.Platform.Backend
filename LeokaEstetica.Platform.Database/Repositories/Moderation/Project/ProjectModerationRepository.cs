using System.Data;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Project;

/// <summary>
/// Класс реализует методы репозитория модерации проектов.
/// </summary>
internal sealed class ProjectModerationRepository : IProjectModerationRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ProjectModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

	#region Публичные методы.

	/// <summary>
	/// Метод получает проект на модерации.
	/// </summary>
	/// /// <param name="projectId">Id проекта.</param>
	public async Task<ModerationProjectEntity> GetProjectModerationAsync(long projectId)
	{
		var result = await _pgContext.ModerationProjects
			.Include(up => up.UserProject)
			.Where(x=>x.ProjectId==projectId)
			.Select(p => new ModerationProjectEntity
			{
				ModerationId = p.ModerationId,
				ProjectId = p.ProjectId,
				UserProject = new UserProjectEntity
				{
					ProjectName = p.UserProject.ProjectName,
					DateCreated = p.UserProject.DateCreated
				},
				DateModeration = p.DateModeration,
                ModerationStatusId=p.ModerationStatusId,
                ModerationStatus=p.ModerationStatus
                
                
			})
			.FirstOrDefaultAsync();

		return result;
	}

	/// <summary>
	/// Метод получает список проектов для модерации.
	/// </summary>
	/// <returns>Список проектов.</returns>
	public async Task<IEnumerable<ModerationProjectEntity>> ProjectsModerationAsync()
    {
        var result = await _pgContext.ModerationProjects
            .Include(up => up.UserProject)
            .Where(p => p.ModerationStatus.StatusId == (int)ProjectModerationStatusEnum.ModerationProject)
            .Select(p => new ModerationProjectEntity
            {
                ModerationId = p.ModerationId,
                ProjectId = p.ProjectId,
                UserProject = new UserProjectEntity
                {
                    ProjectName = p.UserProject.ProjectName,
                    DateCreated = p.UserProject.DateCreated
                },
                DateModeration = p.DateModeration
            })
            .OrderByDescending(p => p.UserProject.DateCreated)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод одобряет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак подиверждения проекта.</returns>
    public async Task<bool> ApproveProjectAsync(long projectId)
    {
        var isSuccessSetStatus = await SetProjectStatusAsync(projectId, ProjectModerationStatusEnum.ApproveProject);

        if (!isSuccessSetStatus)
        {
            return false;
        }

        var isProjectExists = await _pgContext.UserProjects.AnyAsync(v => v.ProjectId == projectId);

        if (!isProjectExists)
        {
            throw new InvalidOperationException($"Не удалось найти проект. ProjectId = {projectId}");
        }
        
        // Проверяем, есть ли уже такой проект в каталоге проектов.
        var isExistsInCatalogProject = await _pgContext.CatalogProjects.AnyAsync(x => x.ProjectId == projectId);
        
        if (!isExistsInCatalogProject)
        {
            // Добавляем проект в каталог.
            await _pgContext.CatalogProjects.AddAsync(new CatalogProjectEntity
            {
                ProjectId = projectId
            });
        }

        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак отклонения проекта.</returns>
    public async Task<bool> RejectProjectAsync(long projectId)
    {
        var result = await SetProjectStatusAsync(projectId, ProjectModerationStatusEnum.RejectedProject);

        return result;
    }

    /// <summary>
    /// Метод отправляет уведомление в приложении при одобрении проекта модератором.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="projectName">Название проекта.</param>
    public async Task AddNotificationApproveProjectAsync(long projectId, long userId, string projectName)
    {
        await _pgContext.Notifications.AddAsync(new NotificationEntity
        {
            ProjectId = projectId,
            VacancyId = null,
            UserId = userId,
            NotificationName = NotificationTypeEnum.ApproveModerationProject.GetEnumDescription(),
            NotificationSysName = NotificationTypeEnum.ApproveModerationProject.ToString(),
            IsNeedAccepted = true,
            Approved = false,
            Rejected = false,
            NotificationText = $"Проект \"{projectName}\" одобрен модератором",
            Created = DateTime.UtcNow,
            NotificationType = NotificationTypeEnum.ApproveModerationProject.ToString(),
            IsShow = true,
            IsOwner = false
        });

        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод отправляет уведомление в приложении при отклонении проекта модератором.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="projectName">Название проекта.</param>
    public async Task AddNotificationRejectProjectAsync(long projectId, long userId, string projectName)
    {
        await _pgContext.Notifications.AddAsync(new NotificationEntity
        {
            ProjectId = projectId,
            VacancyId = null,
            UserId = userId,
            NotificationName = NotificationTypeEnum.RejectModerationProject.GetEnumDescription(),
            NotificationSysName = NotificationTypeEnum.RejectModerationProject.ToString(),
            IsNeedAccepted = true,
            Approved = false,
            Rejected = false,
            NotificationText = $"Проект \"{projectName}\" отклонен модератором",
            Created = DateTime.UtcNow,
            NotificationType = NotificationTypeEnum.RejectModerationProject.ToString(),
            IsShow = true,
            IsOwner = false
        });

        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод создает замечания проекта.
    /// </summary>
    /// <param name="createProjectRemarkInput">Список замечаний.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task CreateProjectRemarksAsync(IEnumerable<ProjectRemarkEntity> projectRemarks)
    {
        await _pgContext.ProjectRemarks.AddRangeAsync(projectRemarks);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает замечания проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний.</returns>
    public async Task<List<ProjectRemarkEntity>> GetProjectRemarksAsync(long projectId)
    {
        var result = await _pgContext.ProjectRemarks
            .Where(pr => pr.ProjectId == projectId
                         && new[]
                         {
                             (int)RemarkStatusEnum.AwaitingCorrection,
                             // (int)RemarkStatusEnum.AgainAssigned,
                             (int)RemarkStatusEnum.NotAssigned,
                             (int)RemarkStatusEnum.Review
                         }.Contains(pr.RemarkStatusId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает замечания проекта, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="fields">Список названий полей..</param>
    /// <returns>Список замечаний.</returns>
    public async Task<List<ProjectRemarkEntity>> GetExistsProjectRemarksAsync(long projectId,
        IEnumerable<string> fields)
    {
        var result = await _pgContext.ProjectRemarks
            .Where(p => p.ProjectId == projectId
                        && fields.Contains(p.FieldName))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет замечания проекта.
    /// </summary>
    /// <param name="projectRemarks">Список замечаний для обновления.</param>
    public async Task UpdateProjectRemarksAsync(List<ProjectRemarkEntity> projectRemarks)
    {
        // TODO: #10343304 Позже надо отрефачить, чтобы не нарушать DRY.
        // Проводим все эти манипуляции, чтобы избежать ошибки при обновлении замечаний, которые уже были внесены.
        foreach (var pr in projectRemarks)
        {
            var local = _pgContext.Set<ProjectRemarkEntity>()
                .Local
                .FirstOrDefault(entry => entry.RemarkId == pr.RemarkId);

            // Если локальная сущность != null.
            if (local != null)
            {
                // Отсоединяем контекст устанавливая флаг Detached.
                _pgContext.Entry(local).State = EntityState.Detached;
            }

            // Проставляем обновляемой сущности флаг Modified.
            _pgContext.Entry(pr).State = EntityState.Modified;
        }

        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод отправляет замечания проекта владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="projectId">Id проекта.</param>
    /// </summary>
    public async Task SendProjectRemarksAsync(long projectId)
    {
        var projectRemarks = await _pgContext.ProjectRemarks
            .Where(pr => pr.ProjectId == projectId)
            .ToListAsync();

        if (projectRemarks.Any())
        {
            foreach (var pr in projectRemarks)
            {
                pr.RemarkStatusId = (int)RemarkStatusEnum.AwaitingCorrection;
            }

            _pgContext.ProjectRemarks.UpdateRange(projectRemarks);
            await _pgContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Метод проверяет, были ли внесены замечания проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак внесения замечаний.</returns>
    public async Task<bool> CheckExistsProjectRemarksAsync(long projectId)
    {
        var result = await _pgContext.ProjectRemarks.AnyAsync(pr => pr.ProjectId == projectId);

        return result;
    }

    /// <summary>
    /// Метод возвращает название проекта по его Id.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns></returns>
    public async Task<string> GetProjectNameAsync(long projectId)
    {
        var projectEntity = await _pgContext.ModerationProjects.FirstOrDefaultAsync(pr => pr.ProjectId == projectId);
        return projectEntity?.UserProject?.ProjectName;
    }

    /// <summary>
    /// Метод получает список замечаний проекта (не отправленные), если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний проекта.</returns>
    public async Task<IEnumerable<ProjectRemarkEntity>> GetProjectUnShippedRemarksAsync(long projectId)
    {
        var result = await _pgContext.ProjectRemarks
            .Where(pr => new[]
                             {
                                 (int)RemarkStatusEnum.NotAssigned,
                                 (int)RemarkStatusEnum.Review
                             }
                             .Contains(pr.RemarkStatusId)
                         && pr.ProjectId == projectId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список замечаний проекта (не отправленные), если они есть.
    /// Выводим эти данные в таблицу замечаний проектов журнала модерации.
    /// </summary>
    /// <returns>Список замечаний проекта.</returns>
    public async Task<IEnumerable<UserProjectEntity>> GetProjectUnShippedRemarksTableAsync()
    {
        var projectRemarksIds = _pgContext.ProjectRemarks
            .Where(pr => new[]
                {
                    (int)RemarkStatusEnum.NotAssigned,
                    (int)RemarkStatusEnum.Review
                }
                .Contains(pr.RemarkStatusId))
            .Select(pr => pr.ProjectId)
            .AsQueryable();

        var result = await (from p in _pgContext.UserProjects
                join pr in _pgContext.ProjectRemarks
                    on p.ProjectId
                    equals pr.ProjectId
                where projectRemarksIds.Contains(pr.ProjectId)
                select new UserProjectEntity
                {
                    ProjectId = pr.ProjectId,
                    ProjectName = p.ProjectName
                })
            .Distinct()
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает проекты, замечания которых ожидают проверки модератором.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<IEnumerable<ProjectRemarkEntity>> GetProjectsAwaitingCorrectionAsync()
    {
        var result = await _pgContext.ProjectRemarks
            .Include(r => r.UserProject)
            .Where(s => s.RemarkStatusId == (int)RemarkStatusEnum.AwaitingCorrection)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает анкеты, замечания которых ожидают проверки модератором.
    /// </summary>
    /// <returns>Список анкет.</returns>
    public async Task<IEnumerable<ResumeRemarkEntity>> GetResumesAwaitingCorrectionAsync()
    {
        var result = await _pgContext.ResumeRemarks
            .Include(r => r.ProfileInfo)
            .Where(s => s.RemarkStatusId == (int)RemarkStatusEnum.AwaitingCorrection)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает вакансии, замечания которых ожидают проверки модератором.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<IEnumerable<VacancyRemarkEntity>> GetVacanciesAwaitingCorrectionAsync()
    {
        var result = await _pgContext.VacancyRemarks
            .Include(r => r.UserProject)
            .Where(s => s.RemarkStatusId == (int)RemarkStatusEnum.AwaitingCorrection)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает комментарии на модерации.
    /// </summary>
    /// <returns>Комментарии на модерации.</returns>
    public async Task<IEnumerable<ProjectCommentModerationEntity>> GetProjectCommentsModerationAsync()
    {
        var result = await _pgContext.ProjectCommentsModeration
            .Include(c => c.ProjectComment)
            .Where(c => new[] { (int)ProjectCommentModerationEnum.ModerationComment }.Contains(c.ModerationStatusId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает комментарий проекта для просмотра.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Данные комментария.</returns>
    public async Task<ProjectCommentModerationEntity> GetCommentModerationByCommentIdAsync(long commentId)
    {
        var result = await _pgContext.ProjectCommentsModeration
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        return result;
    }

    /// <summary>
    /// Метод одобряет комментарий проекта.
    /// </summary>
    /// <param name="commentId">Id комментарии.</param>
    /// <returns>Признак успешного подверждения.</returns>
    public async Task<bool> ApproveProjectCommentAsync(long commentId)
    {
        var comment = await _pgContext.ProjectCommentsModeration
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
        
        if (comment is null)
        {
            return false;
        }
        
        comment.ModerationStatusId = (int)ProjectCommentModerationEnum.ApproveComment;
        await _pgContext.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Метод проверяет, были ли внесены замечания к комментарию проекта.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Признак успешной проверки.</returns>
    public async Task<bool> IfRemarksProjectCommentAsync(long commentId)
    {
        var result = await _pgContext.ProjectCommentsModeration.AnyAsync(c => c.CommentId == commentId);

        return result;
    }

    /// <summary>
    /// Метод отклоняет комментарий проекта.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Признак успешного подверждения.</returns>
    public async Task<bool> RejectProjectCommentAsync(long commentId)
    {
        var comment = await _pgContext.ProjectCommentsModeration
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
        
        if (comment is null)
        {
            return false;
        }
        
        comment.ModerationStatusId = (int)ProjectCommentModerationEnum.RejectedComment;
        await _pgContext.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Метод отправляет проект на модерацию. Это происходит через добавление в таблицу модерации проектов.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    public async Task AddProjectModerationAsync(long projectId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            // Отправляем проект на модерацию.
            var prj = new ModerationProjectEntity
            {
                ProjectId = projectId,
                DateModeration = DateTime.UtcNow,
                ModerationStatusId = (int)ProjectModerationStatusEnum.ModerationProject
            };
            
            // Если проект уже был на модерации, то обновим статус.
            var isModerationExists = await IsModerationExistsProjectAsync(prj.ProjectId);
            
            if (!isModerationExists)
            {
                // Отправляем проект на модерацию.
                await SendModerationProjectAsync(projectId);
            }
            
            else
            {
                await UpdateModerationProjectStatusAsync(projectId, ProjectModerationStatusEnum.ModerationProject);
            }

            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод устанавливает статус проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectModerationStatus">Статус.</param>
    /// <returns>Признак подиверждения проекта.</returns>
    private async Task<bool> SetProjectStatusAsync(long projectId, ProjectModerationStatusEnum projectModerationStatus)
    {
        var prj = await _pgContext.ModerationProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (prj is null)
        {
            throw new InvalidOperationException($"Не удалось найти проект для модерации. ProjectId = {projectId}");
        }

        prj.ModerationStatusId = (int)projectModerationStatus;

        await _pgContext.SaveChangesAsync();

        return true;
    }
    
    /// <summary>
    /// Метод проверяет, был ли уже такой проект на модерации. 
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак модерации.</returns>
    private async Task<bool> IsModerationExistsProjectAsync(long projectId)
    {
        var result = await _pgContext.ModerationProjects
            .AnyAsync(p => p.ProjectId == projectId);

        return result;
    }
    
    /// <summary>
    /// Метод отправляет проект на модерацию.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    private async Task SendModerationProjectAsync(long projectId)
    {
        // Добавляем проект в таблицу модерации проектов.
        await _pgContext.ModerationProjects.AddAsync(new ModerationProjectEntity
        {
            DateModeration = DateTime.UtcNow,
            ProjectId = projectId,
            ModerationStatusId = (int)ProjectModerationStatusEnum.ModerationProject
        });
    }
    
    /// <summary>
    /// Метод обновляет статус проекта на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="status">Статус проекта.</param>
    private async Task UpdateModerationProjectStatusAsync(long projectId, ProjectModerationStatusEnum status)
    {
        var prj = await _pgContext.ModerationProjects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (prj is null)
        {
            throw new InvalidOperationException($"Не найден проект для модерации. ProjectId: {projectId}");
        }
        
        prj.ModerationStatusId = (int)status;
    }

    #endregion
}