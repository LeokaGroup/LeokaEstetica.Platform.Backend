using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Models.Entities.Notification;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Notification;

/// <summary>
/// Класс реализует методы репозитория уведомлений.
/// </summary>
public class NotificationsRepository : INotificationsRepository
{
    private readonly PgContext _pgContext;
    
    public NotificationsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

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
    public async Task<IEnumerable<NotificationEntity>> GetUserProjectsNotificationsAsync(long userId)
    {
        var result = await _pgContext.Notifications
            .Where(n => n.UserId == userId
                        && n.NotificationSysName == NotificationTypeEnum.ProjectInvite.ToString()
                        && n.NotificationType == NotificationTypeEnum.ProjectInvite.ToString()
                        && n.IsShow
                        && n.IsNeedAccepted
                        && !n.Approved
                        && !n.Rejected)
            .ToListAsync();

        return result;
    }
}