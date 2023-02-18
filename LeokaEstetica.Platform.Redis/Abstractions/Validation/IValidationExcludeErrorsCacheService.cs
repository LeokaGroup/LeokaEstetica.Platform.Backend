using LeokaEstetica.Platform.Redis.Models;

namespace LeokaEstetica.Platform.Redis.Abstractions.Validation;

/// <summary>
/// Абстракция сервиса для исключения полей при валидации, которая хранится в кэше.
/// </summary>
public interface IValidationExcludeErrorsCacheService
{
    /// <summary>
    /// Метод получает список полей для исключении при валидации из кэша.
    /// </summary>
    /// <returns>Список полей для исключения.</returns>
    Task<List<ValidationExcludeRedis>> ValidationColumnsExcludeFromCacheAsync();

    /// <summary>
    /// Метод добабвляет в кэш поля для исключения при валидации.
    /// </summary>
    /// <param name="fields">Список полей.</param>
    Task AddValidationColumnsExcludeToCacheAsync(ICollection<ValidationExcludeRedis> fields);

    /// <summary>
    /// Метод обновляет в кэше по ключу.
    /// </summary>
    /// <param name="key">Ключ, по которому нужно обновить.</param>
    Task RefreshCacheAsync(string key);
}