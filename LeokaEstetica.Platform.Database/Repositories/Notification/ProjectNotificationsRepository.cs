using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Models.Entities.Notification;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Notification;

/// <summary>
/// Класс реализует методы репозитория уведомлений проектов.
/// </summary>
public class ProjectNotificationsRepository : IProjectNotificationsRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext"></param>
    public ProjectNotificationsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод записывает уведомление о приглашении пользователя в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    public async Task AddNotificationInviteProjectAsync(long projectId, long? vacancyId, long userId,
        string projectName)
    {
        await _pgContext.Notifications.AddAsync(new NotificationEntity
        {
            ProjectId = projectId,
            VacancyId = vacancyId,
            UserId = userId,
            NotificationName = NotificationTypeEnum.ProjectInvite.GetEnumDescription(),
            NotificationSysName = NotificationTypeEnum.ProjectInvite.ToString(),
            IsNeedAccepted = true,
            Approved = false,
            Rejected = false,
            NotificationText = $"Приглашение в проект \"{projectName}\"",
            Created = DateTime.Now,
            NotificationType = NotificationTypeEnum.ProjectInvite.ToString(),
            IsShow = true,
            IsOwner = false
        });
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список уведомлений.</returns>
    public async Task<(List<NotificationEntity> UserNotifications, List<NotificationEntity> OwnerNotifications)>
        GetUserProjectsNotificationsAsync(long userId)
    {
        (List<NotificationEntity> UserNotifications, List<NotificationEntity> OwnerNotifications) result = (
            new List<NotificationEntity>(), new List<NotificationEntity>());
        
        // Уведомления пользователей, в этом списке нет уведомлений владельцев проектов.
        result.UserNotifications = await _pgContext.Notifications
            .Where(n => n.UserId == userId
                        && n.NotificationSysName == NotificationTypeEnum.ProjectInvite.ToString()
                        && n.NotificationType == NotificationTypeEnum.ProjectInvite.ToString()
                        && n.IsShow
                        && n.IsNeedAccepted
                        && !n.Approved
                        && !n.Rejected)
            .ToListAsync();
        
        // Дополняем этот список уведомлениями, которые должен видеть лишь владелец проектов.
        // Смотрим проекты пользователя, которые находятся в каталоге,
        // так как проекты, которых нет в каталоге нельзя учитывать тут.
        var userProjects = _pgContext.CatalogProjects
            .Where(p => p.Project.UserId == userId)
            .AsQueryable();
        
        var userProjectsIds = userProjects.Select(p => p.ProjectId);

        var userNotifications = _pgContext.Notifications
            .Where(n => userProjectsIds.Contains((long)n.ProjectId)
                        && n.NotificationSysName == NotificationTypeEnum.ProjectInvite.ToString()
                        && n.NotificationType == NotificationTypeEnum.ProjectInvite.ToString()
                        && n.IsShow
                        && n.IsNeedAccepted
                        && !n.Approved
                        && !n.Rejected)
            .AsQueryable();

        if (userNotifications.Any())
        {
            result.OwnerNotifications.AddRange(userNotifications);   
        }

        return result;
    }

    /// <summary>
    /// Метод проверяет существование уведомления по его Id.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <returns>Признак существования уведомления.</returns>
    public async Task<bool> CheckExistsNotificationByIdAsync(long notificationId)
    {
        var result = await _pgContext.Notifications.AnyAsync(n => n.NotificationId == notificationId);

        return result;
    }

    /// <summary>
    /// Метод апрувит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    public async Task ApproveProjectInviteAsync(long notificationId)
    {
        var notification = await _pgContext.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        if (notification is not null)
        {
            notification.Approved = true;
            notification.Rejected = false;
        }

        await _pgContext.SaveChangesAsync();
    }

    #endregion
}