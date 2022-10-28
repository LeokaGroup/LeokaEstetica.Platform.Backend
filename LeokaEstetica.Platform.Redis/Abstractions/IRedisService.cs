using LeokaEstetica.Platform.Redis.Models;

namespace LeokaEstetica.Platform.Redis.Abstractions;

/// <summary>
/// Абстракция работы с кэшем Redis.
/// </summary>
public interface IRedisService
{
    // TODO: Эти два методы возможно будут нужны. Зависит от того, будет ли достаточно использование Clients.All в SignalR или нет. Возможно потом надо будет удалить этот код.
    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    // Task SaveConnectionIdCacheAsync(string connectionId, string userCode);
    //
    // /// <summary>
    // /// Метод получает ConnectionId подключения для SignalR.
    // /// </summary>
    // /// <param name="key">Ключ поиска.</param>
    // /// <returns>ConnectionId.</returns>
    // Task<string> GetConnectionIdCacheAsync(string key);

    /// <summary>
    /// Метод сохраняет в кэш меню профиля пользователя.
    /// </summary>
    /// <param name="profileMenuRedis">Класс для кэша.</param>
    Task SaveProfileMenuCacheAsync(ProfileMenuRedis profileMenuRedis);
    
    /// <summary>
    /// Метод получает из кэша меню профиля пользователя.
    /// </summary>
    /// <param name="key">Ключ для поиска в кэше.</param>
    Task<ProfileMenuRedis> GetProfileMenuCacheAsync(string key);
}