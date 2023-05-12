using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Commerce;

/// <summary>
/// Класс реализует методы сервиса кэша коммерции.
/// </summary>
public class CommerceRedisService : ICommerceRedisService
{
    private readonly IDistributedCache _redisCache;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redisCache">Сервис кэша.</param>
    public CommerceRedisService(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    /// <summary>
    /// Метод создает заказ в кэше.
    /// </summary>
    /// <param name="key">Ключ добавленного заказа.</param>
    /// <param name="createOrderCache">Модель заказа для хранения в кэше.</param>
    public async Task CreateOrderCacheAsync(string key, CreateOrderCache createOrderCache)
    {
        await _redisCache.SetStringAsync(CacheKeysConsts.DEACTIVATE_USER_ACCOUNTS,
            ProtoBufExtensions.Serialize(createOrderCache),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });
    }

    /// <summary>
    /// Метод получает заказ из кэша.
    /// </summary>
    /// <param name="key">Ключ добавленного заказа.</param>
    /// <returns>Данные заказа из кэша.</returns>
    public async Task<CreateOrderCache> GetOrderCacheAsync(string key)
    {
        var orderCache = await _redisCache.GetStringAsync(key);

        // Нет заказа в кэше.
        if (string.IsNullOrEmpty(orderCache))
        {
            return null;
        }

        var result = ProtoBufExtensions.Deserialize<CreateOrderCache>(orderCache);
        
        if (result is null)
        {
            throw new InvalidOperationException($"Не удалось получить заказ из кэша. Key: {key}");
        }

        return result;
    }
}