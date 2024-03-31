using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
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
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="searchProjectManagementRepository">Репозиторий поиска в модуле УП.</param>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public SearchProjectManagementService(ILogger<SearchProjectManagementService> logger,
     ISearchProjectManagementRepository searchProjectManagementRepository,
     IProjectManagmentRepository projectManagmentRepository,
     IProjectSettingsConfigRepository projectSettingsConfigRepository,
     IUserRepository userRepository)
    {
        _logger = logger;
        _searchProjectManagementRepository = searchProjectManagementRepository;
        _projectManagmentRepository = projectManagmentRepository;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchTaskAsync(string searchText, IEnumerable<long> projectIds,
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
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectAsync(string searchText,
        bool isSearchByProjectTaskId, bool isSearchByTaskName, bool isSearchByTaskDescription, long projectId,
        string account, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings?.AsList();

            if (projectSettingsItems is null
                || !projectSettingsItems.Any()
                || projectSettingsItems.Any(x => x is null))
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);
            
            IEnumerable<SearchAgileObjectOutput> result = null;
            var strategy = new BaseSearchAgileObjectAlgorithm();
            
            // Если нужно искать по Id объекта в рамках проекта.
            if (isSearchByProjectTaskId)
            {
                result = await strategy.SearchSearchAgileObjectByProjectTaskIdAsync(
                    new SearchAgileObjectByProjectTaskIdStrategy(_projectManagmentRepository),
                    searchText.GetProjectTaskIdFromPrefixLink(), projectId, templateId, searchAgileObjectType);
            }

            // Если нужно искать по названию объекта.
            if (isSearchByTaskName)
            {
                result = await strategy.SearchSearchAgileObjectByTaskNameAsync(
                    new SearchAgileObjectByTaskNameStrategy(_projectManagmentRepository), searchText, projectId,
                    templateId, searchAgileObjectType);
            }
            
            // Если нужно искать по описанию объекта.
            if (isSearchByTaskDescription)
            {
                result = await strategy.SearchSearchAgileObjectByTaskDescriptionAsync(
                    new SearchAgileObjectByTaskDescriptionStrategy(_projectManagmentRepository), searchText,
                    projectId, templateId, searchAgileObjectType);
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