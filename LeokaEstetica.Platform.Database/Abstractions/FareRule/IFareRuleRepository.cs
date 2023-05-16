using LeokaEstetica.Platform.Models.Entities.FareRule;

namespace LeokaEstetica.Platform.Database.Abstractions.FareRule;

/// <summary>
/// Абстракция репозитория правил тарифа.
/// </summary>
public interface IFareRuleRepository
{
    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    Task<IEnumerable<FareRuleEntity>> GetFareRulesAsync();

    /// <summary>
    /// Метод получает тариф по его Id.
    /// </summary>
    /// <param name="fareRuleId">Id тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleEntity> GetByIdAsync(long fareRuleId);

    /// <summary>
    /// Метод получает список названий входящих в список Ids тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    Task<List<FareRuleEntity>> GetFareRulesNamesByIdsAsync(IEnumerable<long> fareRuleIds);
    
    /// <summary>
    /// Метод получает тариф по его PublicId.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleEntity> GetByPublicIdAsync(Guid publicId);
}