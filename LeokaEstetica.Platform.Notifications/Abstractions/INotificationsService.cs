namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений.
/// </summary>
public interface INotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotifySuccessSaveAsync(string notifyText, string userCode);

    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    // Task SaveConnectionIdCacheAsync(string connectionId, string userCode);
}