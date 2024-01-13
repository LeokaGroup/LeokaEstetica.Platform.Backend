using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Finder.Abstractions.Project;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Output.Pagination;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Finder.Services.Project;

/// <summary>
/// Класс реализует методы сервиса пагинации проектов.
/// </summary>
public class ProjectPaginationService : BaseIndexRamDirectory, IProjectPaginationService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ProjectPaginationService> _logger;

    public ProjectPaginationService(IProjectRepository projectRepository,
        ILogger<ProjectPaginationService> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Метод пагинации проектов.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список проектов.</returns>
    public async Task<PaginationProjectOutput> GetProjectsPaginationAsync(int page)
    {
        try
        {
            var projects = await _projectRepository.GetFiltersProjectsAsync();
            var result = new PaginationProjectOutput
            {
                IsVisiblePagination = true,
                PaginationInfo = new PaginationInfoOutput(projects.Count, page, PaginationConst.TAKE_COUNT)
            };

            // Получаем все проекты из БД без выгрузки в память.
            ProjectsDocumentLoader.Load(projects, _index, _analyzer);

            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var scoreDocs = CreateScoreDocsBuilder.CreateScoreDocsResult(page, searcher);

            result.Projects = CreateProjectsSearchResultBuilder
                .CreateProjectsSearchResult(scoreDocs, searcher)
                .ToList();

            // Если первая страница и записей менее максимального на странице,
            // то надо скрыть пагинацию, так как смысл в пагинации теряется в этом кейсе.
            // Применяем именно к 1 странице, к последней нет (там это надо показывать).
            if (page == 1 && result.Projects.Count < PaginationConst.TAKE_COUNT)
            {
                result.IsVisiblePagination = false;
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}