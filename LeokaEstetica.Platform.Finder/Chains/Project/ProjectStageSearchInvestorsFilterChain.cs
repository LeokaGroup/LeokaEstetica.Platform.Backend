using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Chains.Project;

/// <summary>
/// Метод фмильтрует проекты по стадии "поиск инвесторов".
/// </summary>
/// <param name="filters">Фильтры.</param>
/// <param name="projects">Список проектов.</param>
/// <returns>Список проектов после фильтрации.</returns>
public class ProjectStageSearchInvestorsFilterChain : BaseProjectsFilterChain
{
    /// <summary>
    /// Метод фмильтрует проекты по стадии "поиск инвесторов".
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <param name="projects">Список проектов.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    public override async Task<IQueryable<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        IOrderedQueryable<CatalogProjectOutput> projects)
    {
        // Если фильтр не по стадии проекта "поиск инвесторов", то передаем следующему по цепочке.
        if (!filters.ProjectStages.Contains(FilterProjectStageTypeEnum.SearchInvestors))
        {
            return await CallNextSuccessor(filters, projects);
        }

        Initialize(projects);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);

        var query = new TermQuery(new Term(ProjectFinderConst.PROJECT_STAGE_SYSNAME,
            FilterProjectStageTypeEnum.SearchInvestors.ToString()));
        var filter = new QueryWrapperFilter(query);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), filter, 20).ScoreDocs;
        var result = CreateProjectsSearchResultBuilder.CreateProjectsSearchResult(searchResults, searcher);
        projects = (IOrderedQueryable<CatalogProjectOutput>)result;

        return await CallNextSuccessor(filters, projects);
    }
}