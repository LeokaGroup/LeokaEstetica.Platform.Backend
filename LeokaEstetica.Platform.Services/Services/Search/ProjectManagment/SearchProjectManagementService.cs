using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Search;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.Search.ProjectManagment;
using LeokaEstetica.Platform.Services.Helpers;
using LeokaEstetica.Platform.Services.Strategies.ProjectManagement.AgileObjectSearch;
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
    private readonly Lazy<IPachcaService> _pachcaService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="searchProjectManagementRepository">Репозиторий поиска в модуле УП.</param>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    public SearchProjectManagementService(ILogger<SearchProjectManagementService> logger,
     ISearchProjectManagementRepository searchProjectManagementRepository,
     IProjectManagmentRepository projectManagmentRepository,
     IProjectSettingsConfigRepository projectSettingsConfigRepository,
     IUserRepository userRepository,
     Lazy<IPachcaService> pachcaService)
    {
        _logger = logger;
        _searchProjectManagementRepository = searchProjectManagementRepository;
        _projectManagmentRepository = projectManagmentRepository;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _userRepository = userRepository;
        _pachcaService = pachcaService;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchTaskAsync(string searchText,
        IEnumerable<long> projectIds, bool isById, bool isByName, bool isByDescription)
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
        string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetBuildProjectSpaceSettingsAsync(userId);
            var projectSettingsItems = projectSettings.Settings?.AsList();

            if (projectSettingsItems is null
                || !projectSettingsItems.Any()
                || projectSettingsItems.Any(x => x is null))
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            List<SearchAgileObjectOutput> result = null;
            var strategy = new BaseSearchAgileObjectAlgorithm();
            var searchConditions = new List<bool>
            {
                isSearchByProjectTaskId, isSearchByTaskName, isSearchByTaskDescription
            };
            long parsedProjectTaskId = 0;
            
            // Разбиваем поисковую строку по пробелам.
            var splitedConditions = searchText.Split(" ");
            
            // Если комбинированный режим поиска (т.е. поиск идет по нескольким или всем критериям).
            if (searchConditions.Count(x => x) > 1)
            {
                var projectTaskPrefix = projectSettingsItems.Find(x =>
                        x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX))!
                    .ParamValue;

                // Если есть признак поиска по Id и если сплитованная строка содержит Id задачи с префиксом.
                // То отделяем Id задачи от название или описания.
                if (isSearchByProjectTaskId && splitedConditions
                        .Select(x => x.Contains(projectTaskPrefix))
                        .Any(x => x))
                {
                    try
                    {
                        // Успешно спарсили Id задачи с префиксом.
                        parsedProjectTaskId = splitedConditions.First().GetProjectTaskIdFromPrefixLink();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Спарсить Id задачи с префиксом не удалось.
                        await _pachcaService.Value.SendNotificationErrorAsync(ex);
                        _logger?.LogError(ex, ex.Message);
                    }
                }

                // Пробуем распарсить как число, вдруг передали просто Id задачи.
                else
                {
                    // Если не получится как число, значит там текст и продолжим выполнять логику.
                    if (long.TryParse(splitedConditions.First(), out parsedProjectTaskId))
                    {
                        parsedProjectTaskId = long.Parse(splitedConditions.First());
                    }
                }
            }
            
            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID))!.ParamValue;
            var templateId = Convert.ToInt32(template);
            
            // Если нужно искать по Id задачи в рамках проекта.
            // Если успешно распарсили Id задачи с префиксом.
            if (isSearchByProjectTaskId || parsedProjectTaskId > 0)
            {
                if (parsedProjectTaskId == 0)
                {
                    parsedProjectTaskId = searchText.GetProjectTaskIdFromPrefixLink();
                }
                
                result = (await strategy.SearchAgileObjectByObjectIdAsync(
                    new SearchAgileObjectByObjectIdStrategy(_projectManagmentRepository), parsedProjectTaskId,
                    projectId, templateId))?.AsList();
            }

            // Если нужно искать по названию задачи.
            if (isSearchByTaskName)
            {
                var joinedSearchText = splitedConditions.Length > 1
                    ? string.Join(" ", splitedConditions.Skip(1))
                    : splitedConditions.First();
                
                // Если поиск по нескольким критериям, то дополняем результаты.
                if (result is not null)
                {
                    try
                    {
                        var resultByObjectName = (await strategy.SearchAgileObjectByObjectNameAsync(
                            new SearchAgileObjectByObjectNameStrategy(_projectManagmentRepository), joinedSearchText,
                            projectId, templateId))?.AsList();

                        if (resultByObjectName is not null && resultByObjectName.Any())
                        {
                            result = result.Union(resultByObjectName).AsList();
                        }
                    }
                    
                    // Если не реализовано, то не стапаем приложение, пропустим такой поиск.
                    catch (NotImplementedException ex)
                    {
                        await _pachcaService.Value.SendNotificationErrorAsync(ex);
                        _logger?.LogError(ex, ex.Message);
                    }
                }
                
                else
                {
                    result = (await strategy.SearchAgileObjectByObjectNameAsync(
                        new SearchAgileObjectByObjectNameStrategy(_projectManagmentRepository), joinedSearchText,
                        projectId, templateId))?.AsList();
                }
            }
            
            // Если нужно искать по описанию задачи.
            if (isSearchByTaskDescription)
            {
                var joinedSearchText = splitedConditions.Length > 1
                    ? string.Join(" ", splitedConditions.Skip(1))
                    : splitedConditions.First();
                
                // Если поиск по нескольким критериям, то дополняем результаты.
                if (result is not null)
                {
                    try
                    {
                        var resultByObjectDescription = (await strategy.SearchAgileObjectByObjectDescriptionAsync(
                            new SearchAgileObjectByObjectDescriptionStrategy(_projectManagmentRepository),
                            joinedSearchText, projectId, templateId))?.AsList();

                        if (resultByObjectDescription is not null && resultByObjectDescription.Any())
                        {
                            result = result.Union(resultByObjectDescription).AsList();
                        }
                    }
                    
                    // Если не реализовано, то не стапаем приложение, пропустим такой поиск.
                    catch (NotImplementedException ex)
                    {
                        await _pachcaService.Value.SendNotificationErrorAsync(ex);
                        _logger?.LogError(ex, ex.Message);
                    }
                }

                else
                {
                    result = (await strategy.SearchAgileObjectByObjectDescriptionAsync(
                        new SearchAgileObjectByObjectDescriptionStrategy(_projectManagmentRepository),
                        joinedSearchText, projectId, templateId))?.AsList();
                }
            }

            return result!.OrderBy(o => o.ProjectTaskId);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}