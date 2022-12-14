using LeokaEstetica.Platform.Redis.Models.Profile;

namespace LeokaEstetica.Platform.Redis.Abstractions.Profile;

/// <summary>
/// Абстракция профиля работы с кэшем Redis.
/// </summary>
public interface IProfileRedisService
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
    Task<ProfileMenuRedis> GetProfileMenuCacheAsync();
}