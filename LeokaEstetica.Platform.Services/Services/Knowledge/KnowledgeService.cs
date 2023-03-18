using LeokaEstetica.Platform.Database.Abstractions.Knowledge;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Knowledge;
using LeokaEstetica.Platform.Services.Abstractions.Knowledge;

namespace LeokaEstetica.Platform.Services.Services.Knowledge;

/// <summary>
/// Класс сервиса реализует методы сервиса БЗ.
/// </summary>
public class KnowledgeService : IKnowledgeService
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly ILogService _logService;
    
    /// <summary>
    /// Конструктор.</param>
    /// <param name="knowledgeRepository">Репозиторий БЗ.</param>
    /// <param name="logService">Сервис логирования.</param>
    /// </summary>
    public KnowledgeService(IKnowledgeRepository knowledgeRepository, 
        ILogService logService)
    {
        _knowledgeRepository = knowledgeRepository;
        _logService = logService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает частые вопросы для лендинга.
    /// </summary>
    /// <returns>Список частых вопросов.</returns>
    public async Task<IEnumerable<KnowledgeLandingOutput>> GetLandingKnowledgeAsync()
    {
        try
        {
            
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    #endregion
}