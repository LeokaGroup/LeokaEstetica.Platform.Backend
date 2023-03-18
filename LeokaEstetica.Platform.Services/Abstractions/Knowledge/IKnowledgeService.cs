using LeokaEstetica.Platform.Models.Dto.Output.Knowledge;

namespace LeokaEstetica.Platform.Services.Abstractions.Knowledge;

/// <summary>
/// Абстракция сервиса БЗ.
/// </summary>
public interface IKnowledgeService
{
    /// <summary>
    /// Метод получает частые вопросы для лендинга.
    /// </summary>
    /// <returns>Список частых вопросов.</returns>
    Task<IEnumerable<KnowledgeLandingOutput>> GetLandingKnowledgeAsync();
}