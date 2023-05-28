namespace LeokaEstetica.Platform.Redis.Abstractions.Notification;

/// <summary>
/// Абстракция уведомлений кэш-сервиса.
/// </summary>
public interface INotificationsRedisService
{
    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="token">Токен пользователя.</param>
    Task AddConnectionIdCacheAsync(string connectionId, string token);
    //
    // /// <summary>
    // /// Метод получает ConnectionId подключения для SignalR.
    // /// </summary>
    // /// <param name="key">Ключ поиска.</param>
    // /// <returns>ConnectionId.</returns>
    Task<string> GetConnectionIdCacheAsync(string key);
}