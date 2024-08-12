using LeokaEstetica.Platform.Models.Dto.Output.Notification;

namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений проектов.
/// </summary>
public interface IProjectNotificationsService
{
    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список уведомлений.</returns>
    Task<NotificationResultOutput> GetUserProjectsNotificationsAsync(string account);

    /// <summary>
    /// Метод апрувит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    Task ApproveProjectInviteAsync(long notificationId, string account);

    /// <summary>
    /// Метод реджектит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    Task RejectProjectInviteAsync(long notificationId, string account);
}