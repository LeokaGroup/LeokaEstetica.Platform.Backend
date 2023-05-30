using LeokaEstetica.Platform.Database.Abstractions.Knowledge;
using LeokaEstetica.Platform.Models.Dto.Output.Knowledge;
using LeokaEstetica.Platform.Services.Abstractions.Knowledge;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Knowledge;

/// <summary>
/// Класс сервиса реализует методы сервиса БЗ.
/// </summary>
public class KnowledgeService : IKnowledgeService
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly ILogger<KnowledgeService> _logger;
    
    /// <summary>
    /// Конструктор.</param>
    /// <param name="knowledgeRepository">Репозиторий БЗ.</param>
    /// <param name="logger">Сервис логирования.</param>
    /// </summary>
    public KnowledgeService(IKnowledgeRepository knowledgeRepository, 
        ILogger<KnowledgeService> logger)
    {
        _knowledgeRepository = knowledgeRepository;
        _logger = logger;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает частые вопросы для лендинга.
    /// </summary>
    /// <returns>Список частых вопросов.</returns>
    public async Task<KnowledgeLandingResult> GetLandingKnowledgeAsync()
    {
        try
        {
            var result = new KnowledgeLandingResult
            {
                Title = "Частые вопросы",
                KnowledgeLanding = await _knowledgeRepository.GetKnowlegeLandingAsync()
            };

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion
}