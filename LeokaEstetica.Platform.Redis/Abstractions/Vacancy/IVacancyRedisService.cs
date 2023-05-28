using LeokaEstetica.Platform.Redis.Models.Vacancy;

namespace LeokaEstetica.Platform.Redis.Abstractions.Vacancy;

/// <summary>
/// Абстракция вакансий работы с кэшем Redis.
/// </summary>
public interface IVacancyRedisService
{
    /// <summary>
    /// Метод сохраняет в кэш меню вакансий.
    /// </summary>
    /// <param name="profileMenuRedis">Класс для кэша.</param>
    Task SaveVacancyMenuCacheAsync(VacancyMenuRedis profileMenuRedis);
    
    /// <summary>
    /// Метод получает из кэша меню вакансий.
    /// </summary>
    Task<VacancyMenuRedis> GetVacancyMenuCacheAsync();
}