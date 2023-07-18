using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Landing;
using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Landing;

/// <summary>
/// Класс реализует методы репозитория лендингов.
/// </summary>
internal sealed class LandingRepository : ILandingRepository
{
    private readonly PgContext _pgContext;
    
    public LandingRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
    public async Task<FonEntity> LandingStartFonAsync()
    {
        var result = await _pgContext.Fons
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает данные для фона предложений платформы.
    /// </summary>
    /// <returns>Данные для фона.</returns>
    public async Task<PlatformOfferEntity> GetPlatformDataAsync()
    {
        var result = await _pgContext.PlatformOffer
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список элементов для фона предложений платформы.
    /// </summary>
    /// <returns>Данные для фона.</returns>
    public async Task<IEnumerable<PlatformOfferItemsEntity>> GetPlatformItemsAsync()
    {
        var result = await _pgContext.PlatformOfferItems
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список таймлайнов.
    /// </summary>
    /// <returns>Список таймлайнов.</returns>
    public async Task<Dictionary<string, List<TimelineEntity>>> GetTimelinesAsync()
    {
        var result = await _pgContext.Timelines
            .GroupBy(g => g.TimelineSysType)
            .Select(s => new
            {
                TimelineSysType = s.Key,
                Timelines = s
                    .Select(p => p)
                    .OrderBy(o => o.Position)
                    .ToList()
            })
            .ToDictionaryAsync(k => k.TimelineSysType, v => v.Timelines);

        return result;
    }

    /// <summary>
    /// Метод получает преимущества платформы.
    /// </summary>
    /// <returns>Преимущества платформы.</returns>
    public async Task<IEnumerable<PlatformConditionEntity>> GetPlatformConditionsAsync()
    {
        var result = await _pgContext.PlatformConditions
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }
}