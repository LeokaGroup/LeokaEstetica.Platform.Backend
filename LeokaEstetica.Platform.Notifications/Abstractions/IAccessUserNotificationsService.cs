namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений доступа пользователя.
/// </summary>
public interface IAccessUserNotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление о предупреждении не заполненной анкеты пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotificationWarningEmptyUserProfileAsync(string title, string notifyText, string notificationLevel,
        string token);
}