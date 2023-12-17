using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Chains.Project;

/// <summary>
/// Класс фильтра проектов по дате.
/// </summary>
public class DateProjectsFilterChain : BaseProjectsFilterChain
{
    /// <summary>
    /// Метод фмильтрует проекты по дате.
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <param name="projects">Список проектов.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    public override async Task<List<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        List<CatalogProjectOutput> projects)
    {
        // Если фильтр не по дате, то передаем следующему по цепочке.
        if (Enum.Parse<FilterProjectDateTypeEnum>(filters.Date) != FilterProjectDateTypeEnum.Date)
        {
            return await CallNextSuccessor(filters, projects);
        }
        
        Initialize(projects);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);

        // Если false, то по возрастанию (по дефолту), если true, то по убыванию.
        var sort = new Sort(SortField.FIELD_SCORE,
            new SortField(ProjectFinderConst.DATE_CREATED, SortField.STRING, true));

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), null, 20, sort).ScoreDocs;
        var result = CreateProjectsSearchResultBuilder.CreateProjectsSearchResult(searchResults, searcher);

        return await CallNextSuccessor(filters, result);
    }
}