using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Project;

/// <summary>
/// Класс реализует методы репозитория модерации проектов.
/// </summary>
public class ProjectModerationRepository : IProjectModerationRepository
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
        var isSuccessSetStatus = await SetProjectStatus(projectId, ProjectModerationStatusEnum.ApproveProject);
        
        if (!isSuccessSetStatus)
        {
            return false;
        }
        
        var project = await _pgContext.UserProjects
            .FirstOrDefaultAsync(v => v.ProjectId == projectId);

        if (project is null)
        {
            throw new InvalidOperationException($"Не удалось найти проект. ProjectId = {projectId}");
        }
        
        // Добавляем проект в каталог.
        await _pgContext.CatalogProjects.AddAsync(new CatalogProjectEntity
        {
            ProjectId = projectId
        });
        
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
        var result = await SetProjectStatus(projectId, ProjectModerationStatusEnum.RejectedProject);

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
            Created = DateTime.Now,
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
            Created = DateTime.Now,
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
                             (int)RemarkStatusEnum.AgainAssigned
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
    /// <param name="userId">Id пользователя.</param>
    /// </summary>
    public async Task SendProjectRemarksAsync(long projectId, long userId)
    {
        var projectRemarks = await _pgContext.ProjectRemarks
            .Where(pr => pr.ProjectId == projectId
                         && pr.ModerationUserId == userId)
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
    /// Метод получает список комментариев проекта для модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев.</returns>
    public async Task<IEnumerable<ProjectCommentModerationEntity>> GetProjectCommentsModerationAsync(long projectId)
    {
        var result = await _pgContext.ProjectCommentsModeration
            .Include(p => p.ProjectComment)
            .Where(p => p.ProjectComment.ProjectId == projectId && 
            p.ModerationStatuses.StatusId == (int)CommentModerationStatusEnum.ModerationComment)
            .ToListAsync();

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод устанавливает статус проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectModerationStatus">Статус.</param>
    /// <returns>Признак подиверждения проекта.</returns>
    private async Task<bool> SetProjectStatus(long projectId, ProjectModerationStatusEnum projectModerationStatus)
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

    #endregion
}