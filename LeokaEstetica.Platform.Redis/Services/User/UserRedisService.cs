using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.User;

/// <summary>
/// Класс реализует методы сервиса кэша пользователей.
/// </summary>
public class UserRedisService : IUserRedisService
{
    private readonly IDistributedCache _redisCache;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redisCache">Кэш редиса.</param>
    public UserRedisService(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    /// <summary>
    /// Метод добавляет в кэш Id и токен пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task AddUserTokenAndUserIdCacheAsync(long userId, string token)
    {
        // var toRedis = token + ":" + userId;
        await _redisCache.SetStringAsync(token, ProtoBufExtensions.Serialize(userId.ToString()),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    /// <summary>
    /// Метод получает Id пользователя из кэша.
    /// </summary>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Id пользователя из кэша.</returns>
    public async Task<string> GetUserIdCacheAsync(string token)
    {
        var redisResult = await _redisCache.GetStringAsync(string.Concat(CacheKeysConsts.ADD_CONNECTION_ID, token));
    
        if (string.IsNullOrEmpty(redisResult))
        {
            throw new InvalidOperationException($"Не удалось получить Id пользователя из кэша. Токен: {token}");
        }
        
        var result = ProtoBufExtensions.Deserialize<string>(redisResult);

        if (string.IsNullOrEmpty(result))
        {
            throw new InvalidOperationException($"Не удалось получить Id пользователя из кэша. Токен: {token}");
        }
            
        // Данные нашли, продлеваем время жизни ключа.
        await _redisCache.RefreshAsync(token);
    
        return result;
    }
}