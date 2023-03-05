using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Notification;

/// <summary>
/// Класс реализует методы сервиса уведомлений кэша.
/// </summary>
public class NotificationsRedisService : INotificationsRedisService
{
    private readonly IDistributedCache _redisCache;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redisCache">Кэш редиса.</param>
    public NotificationsRedisService(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task AddConnectionIdCacheAsync(string connectionId, long userId)
    {
        await _redisCache.SetStringAsync(string.Concat(CacheKeysConsts.ADD_CONNECTION_ID, userId.ToString()),
            ProtoBufExtensions.Serialize(connectionId),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    /// <summary>
    /// Метод получает ConnectionId подключения для SignalR.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>ConnectionId.</returns>
    public async Task<string> GetConnectionIdCacheAsync(string key)
    {
        var connectionId = await _redisCache.GetStringAsync(string.Concat(CacheKeysConsts.ADD_CONNECTION_ID, key));

        if (!string.IsNullOrEmpty(connectionId))
        {
            var result = ProtoBufExtensions.Deserialize<string>(connectionId);

            return result;
        }

        return null;
    }
}