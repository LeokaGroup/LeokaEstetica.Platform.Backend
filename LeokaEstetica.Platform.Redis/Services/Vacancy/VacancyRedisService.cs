using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Redis.Abstractions.Vacancy;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Redis.Models.Vacancy;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса вакансий работы с кэшем Redis.
/// </summary>
public sealed class VacancyRedisService : IVacancyRedisService
{
    private readonly IDistributedCache _redis;
    
    public VacancyRedisService(IDistributedCache redis)
    {
        _redis = redis;
    }

    /// <summary>
    /// Метод сохраняет в кэш меню вакансий.
    /// </summary>
    /// <param name="vacancyMenu">Класс для кэша.</param>
    public async Task SaveVacancyMenuCacheAsync(VacancyMenuRedis vacancyMenu)
    {
        await _redis.SetStringAsync(GlobalConfigKeysCache.VACANCY_MENU_KEY,
            ProtoBufExtensions.Serialize(vacancyMenu),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
    }

    /// <summary>
    /// Метод получает из кэша меню вакансий.
    /// </summary>
    public async Task<VacancyMenuRedis> GetVacancyMenuCacheAsync()
    {
        var items = await _redis.GetStringAsync(GlobalConfigKeysCache.VACANCY_MENU_KEY);

        if (string.IsNullOrEmpty(items))
        {
            return new VacancyMenuRedis();
        }
        
        var result = ProtoBufExtensions.Deserialize<VacancyMenuRedis>(items);

        return result;
    }
}