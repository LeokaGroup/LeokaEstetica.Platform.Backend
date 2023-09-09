namespace LeokaEstetica.Platform.Redis.Abstractions.Connection;

/// <summary>
/// Абстракция подключений Redis.
/// </summary>
public interface IConnectionService
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