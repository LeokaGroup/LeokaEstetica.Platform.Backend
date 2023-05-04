namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений.
/// </summary>
public interface INotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifySuccessSaveAsync(string title, string notifyText, string notificationLevel, string token);

    /// <summary>
    /// Метод отправляет уведомление о том, что пользователь не находится в ЧС.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifyWarningUserNotFoundBlackListAsync(string title, string notifyText, string notificationLevel,
        string token);

    /// <summary>
    /// Метод отправляет уведомление о том, что пользователь успешно удален из ЧС.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifySuccessUserRemovedBlackListAsync(string title, string notifyText, string notificationLevel,
        string token);
}