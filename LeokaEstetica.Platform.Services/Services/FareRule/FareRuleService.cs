using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Services.Abstractions.FareRule;

namespace LeokaEstetica.Platform.Services.Services.FareRule;

/// <summary>
/// Класс реализует методы сервиса правил тарифов.
/// </summary>
public class FareRuleService : IFareRuleService
{
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly ILogService _logService;
    
    public FareRuleService(IFareRuleRepository fareRuleRepository, 
        ILogService logService)
    {
        _fareRuleRepository = fareRuleRepository;
        _logService = logService;
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
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}