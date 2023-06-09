using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
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
    public async Task<IEnumerable<FareRuleEntity>> GetFareRulesAsync()
    {
        try
        {
            var result = await _fareRuleRepository.GetFareRulesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}