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
}