using LeokaEstetica.Platform.Models.Dto.Output.Knowledge;

namespace LeokaEstetica.Platform.Database.Abstractions.Knowledge;

/// <summary>
/// Абстракция репозитория БЗ.
/// </summary>
public interface IKnowledgeRepository
{
    Task<IEnumerable<KnowledgeLandingOutput>> GetKnowlegeLandingAsync();
}