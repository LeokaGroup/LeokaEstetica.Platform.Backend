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
    
    /// <summary>
    /// Метод получает тариф по его PublicId.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleCompositeOutput> GetFareRuleByPublicIdAsync(Guid publicId);
}