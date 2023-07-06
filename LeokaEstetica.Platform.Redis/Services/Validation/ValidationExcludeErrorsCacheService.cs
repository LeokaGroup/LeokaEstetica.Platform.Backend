using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Redis.Abstractions.Validation;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Redis.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Validation;

/// <summary>
/// Класс реализует методы сервиса для исключения полей при валидации, которая хранится в кэше.
/// </summary>
public sealed class ValidationExcludeErrorsCacheService : IValidationExcludeErrorsCacheService
{
    private readonly IDistributedCache _redis;
    
    public ValidationExcludeErrorsCacheService(IDistributedCache redis)
    {
        _redis = redis;
    }

    /// <summary>
    /// Метод получает список полей для исключении при валидации из кэша.
    /// </summary>
    /// <returns>Список полей для исключения.</returns>
    public async Task<List<ValidationExcludeRedis>> ValidationColumnsExcludeFromCacheAsync()
    {
        var items = await _redis.GetStringAsync(CacheConst.Cache.VALIDATION_EXCLUDE_KEY);

        if (string.IsNullOrEmpty(items))
        {
            return null;
        }
        
        var result = ProtoBufExtensions.Deserialize<List<ValidationExcludeRedis>>(items);

        return result;
    }

    /// <summary>
    /// Метод добабвляет в кэш поля для исключения при валидации.
    /// </summary>
    /// <param name="fields">Список полей.</param>
    public async Task AddValidationColumnsExcludeToCacheAsync(ICollection<ValidationExcludeRedis> fields)
    {
        await _redis.SetStringAsync(CacheConst.Cache.VALIDATION_EXCLUDE_KEY,
            ProtoBufExtensions.Serialize(fields),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
    }

    /// <summary>
    /// Метод обновляет в кэше по ключу.
    /// </summary>
    /// <param name="key">Ключ, по которому нужно обновить.</param>
    public async Task RefreshCacheAsync(string key)
    {
        await _redis.RefreshAsync(key);
    }
}