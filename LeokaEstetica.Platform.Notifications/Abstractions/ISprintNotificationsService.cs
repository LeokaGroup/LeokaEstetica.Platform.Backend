namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений спринтов.
/// </summary>
public interface ISprintNotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном планировании спринта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifySuccessPlaningSprintAsync(string title, string notifyText, string notificationLevel, string token);
}