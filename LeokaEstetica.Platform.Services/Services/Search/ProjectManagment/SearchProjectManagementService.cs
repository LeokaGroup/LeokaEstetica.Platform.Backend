using LeokaEstetica.Platform.Database.Abstractions.Search;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.Search.ProjectManagment;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Search.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса поиска в модуле управления проектами.
/// </summary>
internal sealed class SearchProjectManagementService : ISearchProjectManagementService
{
    private readonly ILogger<SearchProjectManagementService> _logger;
    private readonly ISearchProjectManagementRepository _searchProjectManagementRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="searchProjectManagementRepository">Репозиторий поиска в модуле УП.</param>
    public SearchProjectManagementService(ILogger<SearchProjectManagementService> logger,
     ISearchProjectManagementRepository searchProjectManagementRepository)
    {
        _logger = logger;
        _searchProjectManagementRepository = searchProjectManagementRepository;
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
}