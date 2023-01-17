using LeokaEstetica.Platform.Models.Entities.FareRule;

namespace LeokaEstetica.Platform.Services.Abstractions.FareRule;

/// <summary>
/// Абстракция сервиса правил тарифов.
/// </summary>
public interface IFareRuleService
{
    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    Task<IEnumerable<FareRuleEntity>> GetFareRulesAsync();
}