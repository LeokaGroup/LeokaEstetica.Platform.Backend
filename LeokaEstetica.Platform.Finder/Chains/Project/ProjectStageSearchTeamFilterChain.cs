using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Finder.Chains.Project;

/// <summary>
/// Класс фильтра по стадии проекта (поиск команды).
/// </summary>
public class ProjectStageSearchTeamFilterChain : BaseProjectsFilterChain
{
    /// <summary>
    /// Метод фмильтрует проекты по стадии "поиск команды".
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <param name="projects">Список проектов.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    public override async Task<List<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        List<CatalogProjectOutput> projects)
    {
        // Если фильтр не по стадии проекта "поиск команды", то передаем следующему по цепочке.
        if (!filters.ProjectStages.Contains(FilterProjectStageTypeEnum.SearchTeam))
        {
            return await CallNextSuccessor(filters, projects);
        }

        Initialize(projects);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);

        var query = new TermQuery(new Term(ProjectFinderConst.PROJECT_STAGE_SYSNAME,
            FilterProjectStageTypeEnum.SearchTeam.ToString()));
        var filter = new QueryWrapperFilter(query);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), filter, 20).ScoreDocs;
        var result = CreateProjectsSearchResultBuilder.CreateProjectsSearchResult(searchResults, searcher).ToList();

        return await CallNextSuccessor(filters, result);
    }
}