using LeokaEstetica.Platform.Models.Dto.Common.Cache;

namespace LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;

/// <summary>
/// Абстракция сервиса компаний в кэше.
/// </summary>
public interface ICompanyRedisService
{
    /// <summary>
    /// Метод добавляет компанию в кэш.
    /// </summary>
    /// <param name="companyRedis">Входная модель.</param>
    Task SetCompanyAsync(CompanyRedis companyRedis);

    /// <summary>
    /// Метод получает компанию из кэша.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>Выходная модель.</returns>
    Task<CompanyRedis?> GetCompanyFromCacheAsync(string? key);
}