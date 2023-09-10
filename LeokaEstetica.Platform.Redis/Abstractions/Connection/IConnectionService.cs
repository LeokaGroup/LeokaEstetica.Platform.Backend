using LeokaEstetica.Platform.Redis.Models.Chat;

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
    
    /// <summary>
    /// Метод получает ConnectionId подключения для SignalR.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>ConnectionId.</returns>
    Task<string> GetConnectionIdCacheAsync(string key);

    /// <summary>
    /// Метод сохраняет список ConnectionId подключений SignalR в кэш.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="dialogRedis">Данные SignalR для хранения в кэше.</param>
    Task AddDialogMembersConnectionIdsCacheAsync(long dialogId, List<DialogRedis> dialogRedis);
    
    /// <summary>
    /// Метод получает список ConnectionId подключений SignalR из кэша.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>Список ConnectionId.</returns>
    Task<List<DialogRedis>> GetDialogMembersConnectionIdsCacheAsync(string key);
}