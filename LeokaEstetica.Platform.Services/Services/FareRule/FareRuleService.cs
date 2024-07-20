using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Services.Abstractions.FareRule;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.FareRule;

/// <summary>
/// Класс реализует методы сервиса правил тарифов.
/// </summary>
internal sealed class FareRuleService : IFareRuleService
{
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly ILogger<FareRuleService> _logger;
    
    public FareRuleService(IFareRuleRepository fareRuleRepository, 
        ILogger<FareRuleService> logger)
    {
        _fareRuleRepository = fareRuleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    public async Task<IEnumerable<FareRuleCompositeOutput>> GetFareRulesAsync()
    {
        try
        {
            var items = await _fareRuleRepository.GetFareRulesAsync();
            var fareRules = items.FareRules?.AsList();
            var fareRuleAttributes = items.FareRuleAttributes?.AsList();
            var fareRuleAttributeValues = items.FareRuleAttributeValues?.AsList();

            if (fareRules is null || fareRules.Count == 0)
            {
                throw new InvalidOperationException("Ошибка получения тарифов.");
            }
            
            if (fareRuleAttributes is null || fareRuleAttributes.Count == 0)
            {
                throw new InvalidOperationException("Ошибка получения атрибутов тарифов.");
            }
            
            if (fareRuleAttributeValues is null || fareRuleAttributeValues.Count == 0)
            {
                throw new InvalidOperationException("Ошибка получения значений атрибутов тарифов.");
            }

            var result = new List<FareRuleCompositeOutput>();
            
            // Заполняем атрибутами каждый тариф.
            foreach (var fr in fareRules)
            {
                var attributes = (from av in fareRuleAttributeValues
                        join a in fareRuleAttributes
                            on av.AttributeId equals a.AttributeId
                        where av.RuleId == fr.RuleId
                        select a)
                    ?.AsList();

                if (attributes is null || attributes.Count == 0)
                {
                    throw new InvalidOperationException("Ошибка заполнения атрибутов тарифа. " +
                                                        $"FareRuleId: {fr.RuleId}.");
                }
                
                // Заполняем значения атрибута тарифа.
                foreach (var attr in attributes)
                {
                    var attributeValues = fareRuleAttributeValues
                        .Where(x => x.RuleId == fr.RuleId
                                    && attr.AttributeId == x.AttributeId)
                        ?.AsList();

                    if (attributeValues is null || attributeValues.Count == 0)
                    {
                        throw new InvalidOperationException("Ошибка заполнения значениями атрибута тарифа. " +
                                                            $"FareRuleId: {fr.RuleId}. " +
                                                            $"AttributeId: {attr.AttributeId}.");
                    }
                    
                    attr.FareRuleAttributeValues.AsList().AddRange(attributeValues);
                }

                // Заполняем атрибуты тарифа.
                fr.FareRuleAttributes.AsList().AddRange(attributes);

                // Заполняем тариф.
                result.Add(fr);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}