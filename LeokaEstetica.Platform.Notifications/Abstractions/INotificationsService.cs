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
    /// <param name="userId">Id пользователя.</param>
    Task SendNotifySuccessSaveAsync(string title, string notifyText, string notificationLevel, long userId);
}