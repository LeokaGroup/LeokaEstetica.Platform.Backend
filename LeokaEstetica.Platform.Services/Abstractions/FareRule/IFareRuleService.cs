using LeokaEstetica.Platform.Models.Dto.Output.FareRule;

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
    Task<IEnumerable<FareRuleCompositeOutput>> GetFareRulesAsync();
}