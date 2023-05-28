using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Finder.Abstractions.Project;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Services.Project;

/// <summary>
/// Класс реализует методы поискового сервиса проектов.
/// </summary>
public class ProjectFinderService : BaseIndexRamDirectory, IProjectFinderService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogService _logService;
    
    public ProjectFinderService(IProjectRepository projectRepository, 
        ILogService logService)
    {
        _projectRepository = projectRepository;
        _logService = logService;
    }

    /// <summary>
    /// Метод находит проекты по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Строка поиска.</param>
    /// <returns>Список проектов соответствующие поисковому запросу.</returns>
    public async Task<CatalogProjectResultOutput> SearchProjectsAsync(string searchText)
    {
        try
        {
            var result = new CatalogProjectResultOutput();
            var projects = await _projectRepository.GetFiltersProjectsAsync();
            
            // Получаем все проекты из БД без выгрузки в память.
            ProjectsDocumentLoader.Load(projects, _index, _analyzer);

            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var queryParser = new QueryParser(_version, ProjectFinderConst.SEARCH_FIELD, _analyzer);
            var query = queryParser.Parse(searchText);

            // Больше 20 и не надо, так как есть пагинация.
            var searchResults = searcher.Search(query, 20).ScoreDocs;
            var items = CreateProjectsSearchResultBuilder.CreateProjectsSearchResult(searchResults, searcher);
            result.CatalogProjects = (IOrderedQueryable<CatalogProjectOutput>)items;

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}