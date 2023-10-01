using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Chains.Project;

/// <summary>
/// Класс фильтра по стадии проекта (тестирование).
/// </summary>
public class ProjectStageTestingFilterChain : BaseProjectsFilterChain
{
    /// <summary>
    /// Класс фильтра по стадии проекта (тестирование).
    /// </summary>
    public override async Task<IQueryable<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        IOrderedQueryable<CatalogProjectOutput> projects)
    {
        // Если фильтр не по стадии проекта "тестирование", то передаем следующему по цепочке.
        if (!filters.ProjectStages.Contains(FilterProjectStageTypeEnum.Testing))
        {
            return await CallNextSuccessor(filters, projects);
        }

        Initialize(projects);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);

        var query = new TermQuery(new Term(ProjectFinderConst.PROJECT_STAGE_SYSNAME,
            FilterProjectStageTypeEnum.Testing.ToString()));
        var filter = new QueryWrapperFilter(query);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), filter, 20).ScoreDocs;
        var result = CreateProjectsSearchResultBuilder.CreateProjectsSearchResult(searchResults, searcher);
        projects = (IOrderedQueryable<CatalogProjectOutput>)result;

        return await CallNextSuccessor(filters, projects);
    }
}