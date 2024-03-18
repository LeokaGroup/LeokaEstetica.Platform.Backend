using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Search;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.Search.ProjectManagment;
using LeokaEstetica.Platform.Services.Helpers;
using LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Search.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса поиска в модуле управления проектами.
/// </summary>
internal sealed class SearchProjectManagementService : ISearchProjectManagementService
{
    private readonly ILogger<SearchProjectManagementService> _logger;
    private readonly ISearchProjectManagementRepository _searchProjectManagementRepository;
    private readonly IProjectManagmentRepository _projectManagmentRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="searchProjectManagementRepository">Репозиторий поиска в модуле УП.</param>
    /// <param name="searchProjectManagementRepository">Репозиторий модуля УП.</param>
    public SearchProjectManagementService(ILogger<SearchProjectManagementService> logger,
     ISearchProjectManagementRepository searchProjectManagementRepository,
     IProjectManagmentRepository projectManagmentRepository)
    {
        _logger = logger;
        _searchProjectManagementRepository = searchProjectManagementRepository;
        _projectManagmentRepository = projectManagmentRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SearchTaskOutput>> SearchTaskAsync(string searchText, IEnumerable<long> projectIds,
        bool isById, bool isByName, bool isByDescription)
    {
        try
        {
            long? projectTaskId = null;
            
            // Если нужен поиск по Id задачи, то из префиксного Id задачи получим чистый.
            if (isById)
            {
                if (!long.TryParse(searchText, out _))
                {
                    // Не можем спарсить число, будем игнорировать при поиске номер задачи.
                    isById = false;
                }

                else
                {
                    projectTaskId = long.Parse(searchText);
                }
            }

            var result = await _searchProjectManagementRepository.SearchTaskAsync(searchText, projectIds, isById,
                isByName, isByDescription, projectTaskId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskAsync(string searchText,
        bool isSearchByProjectTaskId, bool isSearchByTaskName, bool isSearchByTaskDescription, long projectId)
    {
        try
        {
            IEnumerable<SearchTaskOutput> result = null;
            var strategy = new BaseSearchSprintTaskAlgorithm();
            
            // Если нужно искать по Id задачи в рамках проекта.
            if (isSearchByProjectTaskId)
            {
                result = await strategy.SearchIncludeSprintTaskByProjectTaskIdAsync(
                    new SearchIncludeSprintTaskByProjectTaskIdStrategy(_projectManagmentRepository),
                    searchText.GetProjectTaskIdFromPrefixLink(), projectId);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}