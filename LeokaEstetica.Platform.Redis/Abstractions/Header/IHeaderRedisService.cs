using LeokaEstetica.Platform.Models.Dto.Common.Cache;

namespace LeokaEstetica.Platform.Services.Abstractions.Header;

/// <summary>
/// Абстракция сервиса работы с кэшем меню хидера.
/// </summary>
public interface IHeaderRedisService
{
    /// <summary>
    /// Метод сохраняет в кэш меню хидера.
    /// </summary>
    /// <param name="profileMenuRedis">Класс для кэша.</param>
    Task SaveHeaderMenuCacheAsync(List<HeaderMenuRedis> headerMenuRedis);
    
    /// <summary>
    /// Метод получает из кэша меню хидера.
    /// </summary>
    Task<List<HeaderMenuRedis>> GetHeaderMenuCacheAsync();

}