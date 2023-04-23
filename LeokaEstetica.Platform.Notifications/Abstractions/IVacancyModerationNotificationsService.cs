namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстрация сервиса уведомлений модераторов.
/// </summary>
public interface IVacancyModerationNotificationsService
{
    /// <summary>
    /// Отправляет уведомление модератору.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="message">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен модератора.</param>
    public Task SendNotificationWarningApproveVacancyAsync(string title, string message, string notificationLevel, 
        string token);
}