using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Knowledge;
using LeokaEstetica.Platform.Models.Dto.Output.Knowledge;
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

    public async Task<IEnumerable<KnowledgeLandingOutput>> GetKnowlegeLandingAsync()
    {
        var startIds = _pgContext.KnowledgeStart
            .Select(s => s.StartId)
            .AsQueryable();

        var result = await (from c in _pgContext.KnowledgeCategories
                join csc in _pgContext.KnowledgeSubCategories
                    on c.CategoryId
                    equals csc.CategoryId
                join sct in _pgContext.KnowledgeSubCategoryThemes
                    on csc.SubCategoryThemeId
                    equals sct.SubCategoryThemeId
                where startIds.Contains(c.StartId)
                select new KnowledgeLandingOutput
                {
                    SubCategoryThemeTitle = csc.KnowledgeSubCategoryTheme.SubCategoryThemeTitle,
                    SubCategoryThemeText = csc.KnowledgeSubCategoryTheme.SubCategoryThemeText,
                    SubCategoryThemeId = csc.KnowledgeSubCategoryTheme.SubCategoryThemeId
                })
            .OrderBy(o => o.SubCategoryThemeId)
            .ToListAsync();

        return result;
    }

    #endregion
}