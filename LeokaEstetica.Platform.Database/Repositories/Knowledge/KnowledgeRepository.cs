using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Knowledge;
using LeokaEstetica.Platform.Models.Entities.Knowlege;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Knowledge;

/// <summary>
/// Класс реализует методы репозитория БЗ.
/// </summary>
public class KnowledgeRepository : IKnowledgeRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// <param name="pgContext">Датаконтекст.</param>
    /// </summary>
    public KnowledgeRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает данные из стартовой таблицы БЗ.
    /// </summary>
    /// <returns>Данные из стартовой таблицы БЗ.</returns>
    public async Task<List<KnowledgeStartEntity>> GetKnowlegeStartAsync()
    {
        var result = await _pgContext.KnowledgeStart.ToListAsync();

        return result;
    }

    #endregion
}