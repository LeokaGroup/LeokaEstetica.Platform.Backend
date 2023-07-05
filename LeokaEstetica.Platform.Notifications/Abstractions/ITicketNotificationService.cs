namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений тикетов.
/// </summary>
public interface ITicketNotificationService
{
    /// <summary>
    /// Метод отправляет уведомление об ошибке при закрытии тикета.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotificationErrorCloseTicketAsync(string title, string notifyText, string notificationLevel,
        string token);
}