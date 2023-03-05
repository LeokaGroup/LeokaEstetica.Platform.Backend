using LeokaEstetica.Platform.Redis.Models.Profile;

namespace LeokaEstetica.Platform.Redis.Abstractions.Profile;

/// <summary>
/// Абстракция профиля работы с кэшем Redis.
/// </summary>
public interface IProfileRedisService
{
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