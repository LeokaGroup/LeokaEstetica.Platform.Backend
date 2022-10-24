namespace LeokaEstetica.Platform.Redis.Abstractions;

/// <summary>
/// Абстракция работы с кэшем Redis.
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SaveConnectionIdCacheAsync(string connectionId, string userCode);

    /// <summary>
    /// Метод получает ConnectionId подключения для SignalR.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>ConnectionId.</returns>
    Task<string> GetConnectionIdCacheAsync(string key);
}