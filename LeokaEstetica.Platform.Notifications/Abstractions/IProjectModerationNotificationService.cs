namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений модерации проектов.
/// </summary>
public interface IProjectModerationNotificationService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении замечаний проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotificationSuccessCreateProjectRemarksAsync(string title, string notifyText, string notificationLevel,
        string token);
}