using LeokaEstetica.Platform.Models.Entities.Knowlege;

namespace LeokaEstetica.Platform.Database.Abstractions.Knowledge;

/// <summary>
/// Абстракция репозитория БЗ.
/// </summary>
public interface IKnowledgeRepository
{
    /// <summary>
    /// Метод получает данные из стартовой таблицы БЗ.
    /// </summary>
    /// <returns>Данные из стартовой таблицы БЗ.</returns>
    Task<List<KnowledgeStartEntity>> GetKnowlegeStartAsync();
}