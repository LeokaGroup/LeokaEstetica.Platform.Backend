using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.ProjectManagement;

/// <summary>
/// Класс реализует методы сервиса компаний в кэше.
/// </summary>
internal sealed class CompanyRedisService : ICompanyRedisService
{
    private readonly IDistributedCache _redis;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redis">Кэш.</param>
    public CompanyRedisService(IDistributedCache redis)
    {
        _redis = redis;
    }
    
    #region Публичные методы.

    /// <inheritdoc />
    public async Task SetCompanyAsync(CompanyRedis companyRedis)
    {
        var key = CacheConst.Cache.PROJECT_MANAGEMENT_COMPANY_KEY + "_" + companyRedis.CreatedBy;

        await _redis.SetStringAsync(key,
            ProtoBufExtensions.Serialize(companyRedis), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    /// <inheritdoc />
    public async Task<CompanyRedis?> GetCompanyFromCacheAsync(string? key)
    {
        var item = await _redis.GetStringAsync(key);

        if (string.IsNullOrEmpty(item))
        {
            return null;
        }

        var result = ProtoBufExtensions.Deserialize<CompanyRedis?>(item);
        
        // Обновляем время жизни - раз нашли в кэше.
        await _redis.RefreshAsync(key);

        return result;
    }

    #endregion

    #region Приватные методы.

    #endregion
}