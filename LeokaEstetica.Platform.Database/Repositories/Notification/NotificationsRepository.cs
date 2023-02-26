using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Models.Entities.Notification;

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
            NotificationText = $"Приглашение в проект {projectName}",
            Created = DateTime.Now,
            NotificationType = NotificationTypeEnum.ProjectInvite.ToString(),
            IsShow = true
        });
        await _pgContext.SaveChangesAsync();
    }
}