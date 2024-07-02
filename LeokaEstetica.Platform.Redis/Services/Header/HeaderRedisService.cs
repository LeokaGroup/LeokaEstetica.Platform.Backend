using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Services.Abstractions.Header;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Header;

/// <summary>
/// Класс сервиса работы с кэшем меню хидера.
/// </summary>
public class HeaderRedisService : IHeaderRedisService
{
    private readonly IDistributedCache _redis;

    public HeaderRedisService(IDistributedCache redis)
    {
        _redis = redis;
    }
    
    public async Task SaveHeaderMenuCacheAsync(List<HeaderMenuRedis> headerMenuRedis)
    {
        await _redis.SetStringAsync(CacheConst.Cache.HEADER_MENU_KEY,
            ProtoBufExtensions.Serialize(headerMenuRedis),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
    }

    public async Task<List<HeaderMenuRedis>> GetHeaderMenuCacheAsync()
    {
        var items = await _redis.GetStringAsync(CacheConst.Cache.HEADER_MENU_KEY);

        if (string.IsNullOrEmpty(items))
        {
            return new List<HeaderMenuRedis>();
        }
        
        var result = ProtoBufExtensions.Deserialize<List<HeaderMenuRedis>>(items);

        return result;
    }
    
}