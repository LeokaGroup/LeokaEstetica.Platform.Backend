using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Redis.Abstractions.Profile;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Redis.Models.Profile;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Profile;

/// <summary>
/// Класс реализует методы сервиса профиля работы с кэшем Redis.
/// </summary>
public sealed class ProfileRedisService : IProfileRedisService
{
    private readonly IDistributedCache _redis;

    public ProfileRedisService(IDistributedCache redis)
    {
        _redis = redis;
    }

    /// <summary>
    /// Метод сохраняет в кэш меню профиля пользователя.
    /// </summary>
    /// <param name="profileMenuRedis">Класс для кэша.</param>
    public async Task SaveProfileMenuCacheAsync(ProfileMenuRedis profileMenuRedis)
    {
        await _redis.SetStringAsync(GlobalConfigKeys.Cache.PROFILE_MENU_KEY,
            ProtoBufExtensions.Serialize(profileMenuRedis),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
    }

    /// <summary>
    /// Метод получает из кэша меню профиля пользователя.
    /// </summary>param>
    public async Task<ProfileMenuRedis> GetProfileMenuCacheAsync()
    {
        var items = await _redis.GetStringAsync(GlobalConfigKeys.Cache.PROFILE_MENU_KEY);

        if (string.IsNullOrEmpty(items))
        {
            return new ProfileMenuRedis();
        }
        
        var result = ProtoBufExtensions.Deserialize<ProfileMenuRedis>(items);

        return result;
    }
}