using System.Diagnostics;
using System.Runtime.CompilerServices;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Integrations.Abstractions.Reverso;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Factors;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Enum = System.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса управления проектами.
/// </summary>
internal sealed class ProjectManagmentService : IProjectManagmentService
{
    private readonly ILogger<ProjectManagmentService> _logger;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IPachcaService _pachcaService;
    private readonly IProjectManagmentTemplateRepository _projectManagmentTemplateRepository;
    private readonly ITransactionScopeFactory _transactionScopeFactory;
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly Lazy<IReversoService> _reversoService;

    /// <summary>
    /// Статусы задач, которые являются самыми базовыми и никогда не меняются независимо от шаблона проекта.
    /// Новые статусы обязательно должны ассоциироваться с одним из перечисленных системных названий.
    /// </summary>
    private readonly List<string> _associationStatusSysNames = new()
    {
        "New", "InWork", "InDevelopment", "Completed"
    };

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRepository">Репозиторий управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="mapper">Репозиторий пользователей.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="pachcaService">Сервис уведомлений пачки.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов проектов.</param>
    /// <param name="transactionScopeFactory">Факторка транзакций.</param>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="projectSettingsConfigRepository">Сервис транслитера.</param>
    /// </summary>
    public ProjectManagmentService(ILogger<ProjectManagmentService> logger,
        IProjectManagmentRepository projectManagmentRepository,
        IMapper mapper,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IPachcaService pachcaService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        ITransactionScopeFactory transactionScopeFactory,
        IProjectSettingsConfigRepository projectSettingsConfigRepository,
        Lazy<IReversoService> reversoService)
    {
        _logger = logger;
        _projectManagmentRepository = projectManagmentRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _pachcaService = pachcaService;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _transactionScopeFactory = transactionScopeFactory;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _reversoService = reversoService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    public async Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetViewStrategiesAsync();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// Этот метод не заполняет доп.списки.
    /// </summary>
    /// <returns>Список элементов.</returns>
    public async Task<IEnumerable<ProjectManagmentHeaderEntity>> GetHeaderItemsAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetHeaderItemsAsync();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при получении элементов верхнего меню (хидера).");
            throw;
        }
    }

    /// <summary>
    /// Метод наполняет доп.списки элементов хидера.
    /// </summary>
    /// <param name="items">Список элементов.</param>
    public async Task<List<ProjectManagmentHeaderResult>> ModifyHeaderItemsAsync(
        IEnumerable<ProjectManagmentHeaderOutput> items)
    {
        try
        {
            var result = new List<ProjectManagmentHeaderResult>();

            // Будем наполнять доп.списки.
            foreach (var item in items)
            {
                var destinationString = item.Destination;
                var destination = Enum.Parse<ProjectManagmentDestinationTypeEnum>(destinationString);

                if (destination == ProjectManagmentDestinationTypeEnum.None)
                {
                    throw new InvalidOperationException("Неизвестное назначение элемента меню." +
                                                        $"Элемент меню: {destinationString}");
                }

                // Если список стратегий представления.
                if (destination == ProjectManagmentDestinationTypeEnum.Strategy)
                {
                    ProjectManagmentHeaderStrategyItems mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderStrategyItems>(item.Items);
                    }

                    catch (JsonSerializationException ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню стратегий представления." +
                                              $" StrategyItems: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var strategyItems = mapItems.Items.OrderBy(o => o.Position);

                        var selectedStrategyItems = strategyItems.Select(x => new ProjectManagmentHeader
                        {
                            Label = x.ItemName,
                            Id = ProjectManagmentDestinationTypeEnum.Strategy.ToString()
                        });

                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedStrategyItems
                        });
                    }
                }

                // Если список кнопка создания.
                if (destination == ProjectManagmentDestinationTypeEnum.Create)
                {
                    ProjectManagmentHeaderCreateItems mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderCreateItems>(item.Items);
                    }

                    catch (JsonSerializationException ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню кнопки создания." +
                                              $" CreateItems: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var createItems = mapItems.Items.OrderBy(o => o.Position);

                        var selectedCreateItems = createItems.Select(x => new ProjectManagmentHeader
                        {
                            Label = x.ItemName,
                            Id = ProjectManagmentDestinationTypeEnum.Create.ToString()
                        });

                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedCreateItems
                        });
                    }
                }

                // Если фильтры.
                if (destination == ProjectManagmentDestinationTypeEnum.Filters)
                {
                    ProjectManagmentHeaderFilters mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderFilters>(item.Items);
                    }

                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню фильтров." +
                                              $" Filters: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var filters = mapItems.Items.OrderBy(o => o.Position);

                        var selectedFilters = filters.Select(x => new ProjectManagmentHeader
                        {
                            Label = x.ItemName,
                            Id = ProjectManagmentDestinationTypeEnum.Filters.ToString()
                        });

                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedFilters
                        });
                    }
                }

                // Если настройки.
                if (destination == ProjectManagmentDestinationTypeEnum.Settings)
                {
                    ProjectManagmentHeaderFilters mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderFilters>(item.Items);
                    }

                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню настроек." +
                                              $" Filters: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var filters = mapItems.Items.OrderBy(o => o.Position);

                        var selectedFilters = filters.Select(x => new ProjectManagmentHeader
                        {
                            Label = x.ItemName,
                            Id = ProjectManagmentDestinationTypeEnum.Settings.ToString()
                        });

                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedFilters
                        });
                    }
                }
            }

            return await Task.FromResult(result);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при наполнении доп.списка элементов верхнего меню (хидера).");
            throw;
        }
    }

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync(
        long? templateId)
    {
        try
        {
            var items = (await _projectManagmentRepository.GetProjectManagmentTemplatesAsync(templateId))
                .ToList();
            
            // Разбиваем статусы на группы (кажда группа это отдельный шаблон со статусами).
            var templateIds = items.Select(x => x.TemplateId).Distinct().ToList();
            var result = new List<ProjectManagmentTaskTemplateResult>();

            foreach (var tid in templateIds)
            {
                // Выбираем все статусы определенного шаблона и добавляем в результат. 
                var templateStatuses = items.Where(x => x.TemplateId == tid);
                var resultItem = new ProjectManagmentTaskTemplateResult
                {
                    TemplateName = templateStatuses.First().TemplateName,
                    ProjectManagmentTaskStatusTemplates =
                        _mapper.Map<IEnumerable<ProjectManagmentTaskStatusTemplateOutput>>(templateStatuses)
                };
                result.Add(resultItem);
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод проставляет Id шаблонов статусам для результата.
    /// </summary>
    /// <param name="templateStatuses">Список статусов.</param>
    public async Task SetProjectManagmentTemplateIdsAsync(List<ProjectManagmentTaskTemplateResult> templateStatuses)
    {
        try
        {
            if (templateStatuses is null || !templateStatuses.Any())
            {
                throw new InvalidOperationException(
                    "Невозможно проставить Id щаблонов статусам задач." +
                    $" TemplateStatuses: {JsonConvert.SerializeObject(templateStatuses)}");
            }

            // Находим в БД все статусы по их Id.
            var templateStatusIds = templateStatuses
                .SelectMany(x => x.ProjectManagmentTaskStatusTemplates
                    .Select(y => y.StatusId));
            var items = templateStatuses
                .SelectMany(x => x.ProjectManagmentTaskStatusTemplates
                    .Select(y => y));

            var statuses = (await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(templateStatusIds))
                .ToList();

            foreach (var ts in items)
            {
                var statusId = ts.StatusId;
                var templateData = statuses.Find(x => x.StatusId == statusId);

                // Если не нашли такого статуса в таблице маппинга многие-ко-многим.
                if (templateData is null || templateData.StatusId <= 0 || templateData.TemplateId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {statusId}");
                }

                ts.TemplateId = templateData.TemplateId;
            }
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SetProjectManagmentTemplateIdsAsync(List<TaskStatusOutput> templateStatuses)
    {
        try
        {
            if (templateStatuses is null || !templateStatuses.Any())
            {
                throw new InvalidOperationException(
                    "Невозможно проставить Id щаблонов статусам задач." +
                    $" TemplateStatuses: {JsonConvert.SerializeObject(templateStatuses)}");
            }

            // Находим в БД все статусы по их Id.
            var templateStatusIds = templateStatuses.Select(x => x.StatusId);

            var statuses = (await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(templateStatusIds))
                .ToList();

            foreach (var ts in templateStatuses)
            {
                var statusId = ts.StatusId;
                var templateData = statuses.Find(x => x.StatusId == statusId);

                // Если не нашли такого статуса в таблице маппинга многие-ко-многим.
                if (templateData is null || templateData.StatusId <= 0 || templateData.TemplateId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {statusId}");
                }

                ts.TemplateId = templateData.TemplateId;
            }
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает конфигурацию рабочего пространства по выбранному шаблону.
    /// Под конфигурацией понимаются основные элементы рабочего пространства (набор задач, статусов, фильтров, колонок и тд)
    /// если выбранный шаблон это предполагает.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    public async Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(long projectId,
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

            // Проверяем доступ к проекту.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            if (!isOwner)
            {
                throw new InvalidOperationException("Пользователь не является владельцем проекта." +
                                                    $"ProjectId: {projectId}." +
                                                    $"UserId: {userId}");
            }
            
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings?.ToList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            // Получаем набор статусов, которые входят в выбранный шаблон.
            var items = await GetProjectManagmentTemplatesAsync(templateId);
            var templateStatusesItems = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
            var statuses = templateStatusesItems?.ToList();

            if (statuses is null || !statuses.Any())
            {
                throw new InvalidOperationException("Не удалось получить набор статусов шаблона." +
                                                    $" TemplateId: {templateId}." +
                                                    $"ProjectId: {projectId}.");
            }

            // Проставляем Id шаблона статусам.
            await SetProjectManagmentTemplateIdsAsync(statuses);
            
            var strategy = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY));

            var result = new ProjectManagmentWorkspaceResult
            {
                ProjectManagmentTaskStatuses = statuses.First().ProjectManagmentTaskStatusTemplates
                    .Where(x => x.TemplateId == templateId),
                Strategy = strategy!.ParamValue
            };

            // Получаем задачи пользователя, которые принадлежат проекту в рабочем пространстве.
            var projectTasks = await _projectManagmentRepository.GetProjectTasksAsync(projectId);
            var tasks = projectTasks?.ToList();

            if (tasks is not null && tasks.Any())
            {
                var modifyStatusesTimer = new Stopwatch();
                
                _logger.LogInformation(
                    $"Начали получение списка задач для рабочего пространства для проекта {projectId}");
                
                modifyStatusesTimer.Start();

                // Наполняем названиями исходя из Id полей.
                await ModifyProjectManagmentTaskStatusesResultAsync(result.ProjectManagmentTaskStatuses, tasks,
                    projectId);
                
                modifyStatusesTimer.Stop();
                
                _logger.LogInformation(
                    $"Закончили получение списка задач для рабочего пространства для проекта {projectId} " +
                    $"за: {modifyStatusesTimer.ElapsedMilliseconds} мсек.");
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    public async Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync(long projectTaskId, string account,
        long projectId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // TODO: Добавить проверку. Является ли пользователь участником проекта. Если нет, то не давать доступ к задаче.
            var task = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(projectTaskId, projectId);

            // Получаем имя автора задачи.
            var authors = await _userRepository.GetAuthorNamesByAuthorIdsAsync(new[] { task.AuthorId });

            if (authors.Count == 0)
            {
                throw new InvalidOperationException("Не удалось получить автора задачи.");
            }

            var executorId = task.ExecutorId;

            // Обязательно логируем такое если обнаружили, но не стопаем выполнение логики.
            if (executorId <= 0)
            {
                var ex = new InvalidOperationException(
                    "Найден невалидный исполнитель задачи." +
                    $" ExecutorIds: {JsonConvert.SerializeObject(executorId)}.");

                _logger.LogError(ex, ex.Message);

                // Отправляем ивент в пачку.
                await _pachcaService.SendNotificationErrorAsync(ex);
            }

            // Получаем имена исполнителя задачи.
            var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(new[] { executorId });

            if (executors.Count == 0)
            {
                throw new InvalidOperationException("Не удалось получить исполнителей задач.");
            }

            var result = _mapper.Map<ProjectManagmentTaskOutput>(task);
            result.ExecutorName = executors.TryGet(executors.First().Key).FullName;
            result.AuthorName = authors.TryGet(authors.First().Key).FullName;

            var watcherIds = task.WatcherIds;

            // Если есть наблюдатели, пойдем получать их.
            // Если каких то нет, не страшно, значит они не заполнены у задач.
            if (watcherIds is not null && watcherIds.Any())
            {
                var watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);

                // Наблюдатели задачи.
                if (watchers is not null && watchers.Count > 0)
                {
                    var watcherNames = new List<string>();

                    foreach (var w in result.WatcherIds)
                    {
                        watcherNames.Add(watchers.TryGet(w)?.FullName);
                    }

                    result.WatcherNames = watcherNames;
                }
            }

            var tagIds = task.TagIds;

            // Если есть теги, то пойдем получать.
            if (tagIds is not null && tagIds.Any())
            {
                var tags = await _projectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);

                // Название тегов (меток) задачи.
                if (tags is not null && tags.Count > 0)
                {
                    var tagNames = new List<string>();

                    foreach (var tg in result.TagIds)
                    {
                        var tgName = tags.TryGet(tg).TagName;
                        tagNames.Add(tgName);
                    }

                    result.TagNames = tagNames;
                }
            }

            var taskStatusId = task.TaskTypeId;
            var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(new[] { taskStatusId });
            result.TaskTypeName = types.TryGet(result.TaskTypeId).TypeName;

            var statuseName = await _projectManagmentTemplateRepository.GetStatusNameByTaskStatusIdAsync(
                Convert.ToInt32(task.TaskStatusId));

            if (string.IsNullOrEmpty(statuseName))
            {
                throw new InvalidOperationException($"Не удалось получить TaskStatusName: {taskStatusId}.");
            }

            result.TaskStatusName = statuseName;

            var resolutionId = task.ResolutionId;

            // Если есть резолюции задач, пойдем получать их.
            // Если каких то нет, не страшно, значит они не заполнены у задач.
            if (resolutionId.HasValue)
            {
                var resolutions = await _projectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
                    new[] { resolutionId.Value });

                // Получаем резолюцию задачи, если она есть.
                if (resolutions is not null && resolutions.Count > 0)
                {
                    result.ResolutionName = resolutions.TryGet(result.ResolutionId).ResolutionName;
                }
            }

            var priorityId = task.PriorityId;

            // Если есть приоритеты задач, пойдем получать их.
            // Если каких то нет, не страшно, значит они не заполнены у задач.
            if (priorityId.HasValue)
            {
                var priorities = await _projectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                    new[] { priorityId.Value });

                if (priorities is not null && priorities.Count > 0)
                {
                    var priorityName = priorities.TryGet(priorityId.Value);

                    // Если приоритета нет, не страшно. Значит не указана у задачи.
                    if (priorityName is not null)
                    {
                        result.PriorityName = priorities.TryGet(priorityId.Value).PriorityName;
                    }
                }
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task<CreateProjectManagementTaskOutput> CreateProjectTaskAsync(
        CreateProjectManagementTaskInput projectManagementTaskInput, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var projectId = projectManagementTaskInput.ProjectId;
            
            // Находим наибольший Id задачи в рамках проекта и увеличиваем его.
            var maxProjectTaskId = await _projectManagmentRepository.GetLastProjectTaskIdAsync(projectId);

            if (maxProjectTaskId <= 0)
            {
                throw new InvalidOperationException("Не удалось получить наибольший Id задачи в рамках проекта." +
                                                    $"ProjectId: {projectId}");
            }

            ProjectTaskEntity addedProjectTask;

            using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
            
            // Ускоренное создание задачи.
            if (projectManagementTaskInput.IsQuickCreate)
            {
                addedProjectTask = CreateProjectTaskFactory.CreateQuickProjectTask(projectManagementTaskInput, userId);
            }

            // Обычное создание задачи.
            else
            {
                // Если не был указан исполнитель задачи, то исполнителем задачи будет ее автор.
                if (!projectManagementTaskInput.ExecutorId.HasValue)
                {
                    projectManagementTaskInput.ExecutorId = userId;
                }
                
                addedProjectTask = CreateProjectTaskFactory.CreateProjectTask(projectManagementTaskInput, userId);
            }

            addedProjectTask.ProjectTaskId = ++maxProjectTaskId;
            
            // Создаем задачу в БД.
            await _projectManagmentRepository.CreateProjectTaskAsync(addedProjectTask);

            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings?.ToList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            var redirectUrl = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_SPACE_URL));

            var result = new CreateProjectManagementTaskOutput
            {
                RedirectUrl = string.Concat(redirectUrl!.ParamValue, $"?projectId={projectId}")
            };
            
            transactionScope.Complete();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    public async Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetTaskPrioritiesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    public async Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetTaskTypesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectTagEntity>> GetProjectTagsAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetProjectTagsAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskStatusOutput>> GetTaskStatusesAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Получаем все статусы, которые входят в шаблон текущего проекта.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId);
            var projectSettingsItems = projectSettings?.ToList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}.");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            var statusIds = (await _projectManagmentTemplateRepository.GetTemplateStatusIdsAsync(templateId))
                ?.ToList();

            if (statusIds is null || !statusIds.Any())
            {
                throw new InvalidOperationException("Не удалось получить статусы для выбора в задаче." +
                                                    $"ProjectId: {projectId}");
            }

            var items = await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds);
            var result = _mapper.Map<IEnumerable<TaskStatusOutput>>(items).ToList();
        
            // Проставляем Id шаблона статусам.
            await SetProjectManagmentTemplateIdsAsync(result);

            var removedTaskStatus = new List<TaskStatusOutput>();

            foreach (var ts in result)
            {
                // Если шаблон не совпадает с шаблоном текущего проекта, то такой статус не нужен.
                if (ts.TemplateId != templateId)
                {
                    removedTaskStatus.Add(ts);
                }
            }

            // Если есть, что удалять, то удаляем статусы, которые не входят в шаблон проекта.
            if (removedTaskStatus.Any())
            {
                result.RemoveAll(x => removedTaskStatus.Select(y => y.StatusId).Contains(x.StatusId));
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает пользователей, которые могут быть выбраны в качестве исполнителя задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список пользователей.</returns>
    public async Task<IEnumerable<ProfileInfoEntity>> GetSelectTaskExecutorsAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var teamId = await _projectRepository.GetProjectTeamIdByProjectIdAsync(projectId);

            if (teamId <= 0)
            {
                throw new InvalidOperationException($"Не удалось получить команду проекта. ProjectId: {projectId}");
            }

            var teamMemberIds = await _projectRepository.GetProjectTeamMemberIdsAsync(teamId);
            var teamMemberIdsItems = teamMemberIds?.ToList();

            // Если результат null или нету пользователей или есть 1 пользователь и если этот пользователь тот,
            // кто создает сейчас задачу, то список будет пустым.
            // Потому что текущий пользователь может назначить себя исполнителем задачи через кнопку "Назначить меня".
            if (teamMemberIdsItems is null 
                || !teamMemberIdsItems.Any() 
                || (teamMemberIdsItems.Count == 1 && teamMemberIdsItems.First() == userId))
            {
                return Enumerable.Empty<ProfileInfoEntity>();
            }

            var executors = await _userRepository.GetProfileInfoByUserIdsAsync(teamMemberIdsItems);
            var executorsItems = executors?.ToList();

            if (executorsItems is null || !executorsItems.Any())
            {
                return Enumerable.Empty<ProfileInfoEntity>();
            }

            return executorsItems;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateProjectTagAsync(string tagName, string tagDescription, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var tagSysName = await _reversoService.Value.TranslateTextRussianToEnglishAsync(tagName,
                TranslateLangTypeEnum.Russian, TranslateLangTypeEnum.English);

            // Разбиваем строку на пробелы и приводим каждое слово к PascalCase и соединяем снова в строку.
            tagSysName = string.Join("", tagSysName.Split(" ").Select(x => x.ToPascalCase()));

            var maxUserTagPosition = await _projectManagmentRepository.GetLastPositionUserTaskTagAsync(userId);
            
            var userTag = CreateUserTaskTagFactory.CreateProjectTag(tagName, tagDescription, tagSysName,
                    ++maxUserTagPosition, projectId);
            await _projectManagmentRepository.CreateProjectTaskTagAsync(userTag);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetSelectableTaskStatusesAsync(
        long projectId, int templateId)
    {
        try
        {
            var statusIds = await _projectManagmentTemplateRepository.GetTemplateStatusIdsAsync(
                templateId);
            var statusIdsItems = statusIds?.ToList();

            if (statusIdsItems is null || !statusIdsItems.Any())
            {
                throw new InvalidOperationException("Не удалось получить статусы шаблона." +
                                                    $"ProjectId: {projectId}. " +
                                                    $"TemplateId: {templateId}.");
            }

            var result = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIdsItems))
                .ToList();

            // TODO: Лучше сразу получить лишь нужные статусы в методе GetTaskTemplateStatusesAsync выше
            // TODO: сделав отдельным методом на это. Пока переиспользуем, но в будущем отрефачить.
            // Удаляем все лишние статусы, кроме базовых для всех шаблонов проекта.
            result.RemoveAll(x => !_associationStatusSysNames.Contains(x.StatusSysName));

            // Если есть оба системных названия, то оставим одно. InWork имеет приоритет.
            if (result.Find(x => x.StatusSysName.Equals("InWork")) is not null
                && result.Find(x => x.StatusSysName.Equals("InDevelopment")) is not null)
            {
                var removedDevelopment = result.Find(x => x.StatusSysName.Equals("InDevelopment"));
                result.Remove(removedDevelopment);
            }

            // Если несколько системных названий Completed, то оставим одно.
            if (result.Count(x => x.StatusSysName.Equals("Completed")) > 1)
            {
                result = result.DistinctBy(d => d.StatusSysName).ToList();
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateUserTaskStatusTemplateAsync(string associationSysName, string statusName,
        string statusDescription, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var associationStatus = await _projectManagmentTemplateRepository.GetProjectManagementStatusBySysNameAsync(
                associationSysName);

            var associationStatusSysName = associationStatus.StatusSysName;

            // Если системное название статуса недопустимо, то кидаем исключение.
            if (!_associationStatusSysNames.Contains(associationStatusSysName))
            {
                throw new InvalidOperationException("Системное название статуса не определенно как базовый. " +
                                                    "Проверьте маппинг статуса в таблице TaskStatuses" +
                                                    $" StatusSysName: {associationStatusSysName}. " +
                                                    $"AssociationStatusSysName: {associationStatusSysName}. " +
                                                    $"StatusName: {statusName}. " +
                                                    $"ProjectId: {projectId}");
            }

            var lastUserPosition = await _projectManagmentTemplateRepository.GetLastPositionUserStatusTemplateAsync(
                userId);
            
            var statusSysName = await _reversoService.Value.TranslateTextRussianToEnglishAsync(statusName,
                TranslateLangTypeEnum.Russian, TranslateLangTypeEnum.English);

            // Разбиваем строку на пробелы и приводим каждое слово к PascalCase и соединяем снова в строку.
            statusSysName = string.Join("", statusSysName.Split(" ").Select(x => x.ToPascalCase()));

            var addedCustomUserStatus = CreateTaskStatusFactory.CreateUserStatuseTemplate(statusName, statusSysName,
                ++lastUserPosition, userId, statusDescription);

            // Создаем кастомный статус пользователя.
            // Кастомный статус добавляется в шаблон пользователя (расширение шаблона), по которому управляется проект.
            // Исходный шаблон проекта не изменяется и его статусы тоже.
            var addedStatusId = await _projectManagmentTemplateRepository
                .CreateProjectManagmentTaskStatusTemplateAsync(addedCustomUserStatus);

            if (addedStatusId <= 0)
            {
                throw new InvalidOperationException(
                    "Невалидный Id кастомного статуса. " +
                    "Возможно, добавление не произошло. " +
                    "Нужно првоерить таблицу кастомных статусов шаблонов пользователей. " +
                    $"ProjectId: {projectId}. " +
                    $"Id кастомного статуса был: {addedStatusId}");
            }
            
            // TODO: Изменить на получение шаблона из репозитория конфигов настроек проектов.
            // Получаем шаблон проекта, если он был выбран.
            var projectTemplateId = await _projectManagmentTemplateRepository.GetProjectTemplateIdAsync(projectId);

            if (!projectTemplateId.HasValue || projectTemplateId.Value <= 0)
            {
                throw new InvalidOperationException($"Не удалось получить шаблон проекта. ProjectId: {projectId}");
            }
            
            // Добавляем этот новый кастомный статус в маппинг статусов шаблонов.
            await _projectManagmentTemplateRepository.CreateProjectManagmentTaskStatusIntermediateTemplateAsync(
                addedStatusId, projectTemplateId.Value);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AvailableTaskStatusTransitionOutput>> GetAvailableTaskStatusTransitionsAsync(
        long projectId, long projectTaskId)
    {
        try
        {
            var ifProjectHavingTask = await _projectManagmentRepository.IfProjectHavingProjectTaskIdAsync(projectId,
                projectTaskId);

            // Если задача не принадлежит проекту.
            if (!ifProjectHavingTask)
            {
                throw new InvalidOperationException("Задача не принадлежит проекту. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"ProjectTaskId: {projectTaskId}");
            }

            // Получаем текущий статус задачи.
            var currentTaskStatusId = await _projectManagmentRepository
                .GetProjectTaskStatusIdByProjectIdProjectTaskIdAsync(projectId, projectTaskId);

            if (currentTaskStatusId <= 0)
            {
                throw new InvalidOperationException("Не удалось получить текущий статус задачи. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"ProjectTaskId: {projectTaskId}");
            }

            // Получаем все переходы из промежуточной таблицы отталкиваясь от текущего статуса задачи.
            var statusIds = (await _projectManagmentRepository
                    .GetProjectManagementTransitionIntermediateTemplatesAsync(currentTaskStatusId))?.ToList();

            if (statusIds is null || !statusIds.Any())
            {
                throw new InvalidOperationException(
                    $"Не удалось получить доступные переходы для статуса {currentTaskStatusId}. " +
                    $"ProjectId: {projectId}. " +
                    $"ProjectTaskId: {projectTaskId}.");
            }

            // Получаем все статусы по переходам.
            var transitionStatuses = (await _projectManagmentRepository.GetTaskStatusIntermediateTemplatesAsync(
                statusIds))?.ToList();

            if (transitionStatuses is null || !transitionStatuses.Any())
            {
                throw new InvalidOperationException(
                    $"Не удалось получить статусы переходов: {JsonConvert.SerializeObject(transitionStatuses)}. " +
                    $"Id статусов были: {JsonConvert.SerializeObject(statusIds)} " +
                    $"ProjectId: {projectId}. " +
                    $"ProjectTaskId: {projectTaskId}. " +
                    $"Id текущего статуса задачи: {currentTaskStatusId}.");
            }
            
            // Получаем названия статусов у таких переходов.
            // Опираемся на признаки кастомности промежуточных таблиц переходов и статусов
            // IsCustomTransition и IsCustomStatus.
            var commonStatuses = await _projectManagmentRepository.GetTaskStatusTemplatesAsync();
            
            // Если нету общих статусов, но среди переходов был минимум 1 общий,
            // то это ошибка и опасно продолжать дальше. Это может нарушить целостность переходов.
            if (commonStatuses is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить кастомные статусы пользователя," +
                    " хотя был минимум 1 кастомный статус среди: " +
                    $"{JsonConvert.SerializeObject(transitionStatuses)}.");
            }
            
            var userStatuses = await _projectManagmentRepository.GetUserTaskStatusTemplatesAsync();
            
            // Если нету кастомных статусов, но среди переходов был минимум 1 кастомный,
            // то это ошибка и опасно продолжать дальше. Это может нарушить целостность переходов.
            if (userStatuses is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить кастомные статусы пользователя," +
                    " хотя был минимум 1 кастомный статус среди: " +
                    $"{JsonConvert.SerializeObject(transitionStatuses)}.");
            }
            
            // Получаем все статусы, которые входят в шаблон текущего проекта.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId);
            var projectSettingsItems = projectSettings?.ToList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}.");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            // Получаем все Id статусов, которые входят в шаблон текущего проекта.
            var templateStatusIds = (await _projectManagmentTemplateRepository.GetTemplateStatusIdsAsync(templateId))
                ?.ToList();

            if (templateStatusIds is null || !templateStatusIds.Any())
            {
                throw new InvalidOperationException($"Не удалось получить статусы шаблона проекта: {templateId}.");
            }
            
            // Получаем сами статусы, которые входят в шаблон текущего проекта.
            var templateStatuses = (await _projectManagmentTemplateRepository
                .GetTaskTemplateStatusesAsync(templateStatusIds))?.ToDictionary(k => k.StatusId, v => v);

            if (templateStatuses is null || !templateStatuses.Any())
            {
                throw new InvalidOperationException(
                    $"Не удалось получить статусы шаблона проекта: {projectId}. " +
                    $"Id статусов были: {JsonConvert.SerializeObject(templateStatusIds)}.");
            }

            var result = new List<AvailableTaskStatusTransitionOutput>();
            
            foreach (var ts in transitionStatuses)
            {
                // Если шаблон не совпадает с шаблоном текущего проекта, то такой статус не нужен.
                if (ts.TemplateId != templateId)
                {
                    continue;
                }
                
                var statusId = ts.StatusId;
                
                if (statusId <= 0)
                {
                    throw new InvalidOperationException($"Невалидный статус задачи: {statusId}. " +
                                                        $"Id задачи: {projectTaskId}. " +
                                                        $"Id проекта: {projectId}.");
                }

                // Если статус не входит в шаблон проекта, то исключаем его.
                if (!templateStatuses.ContainsKey(statusId))
                {
                    continue;
                }

                // Если кастомный статус, то получать будем из таблицы кастомных статусов.
                if (ts.IsCustomStatus)
                {
                    if (!userStatuses.ContainsKey(statusId))
                    {
                        throw new InvalidOperationException(
                            $"Статуса {statusId} нет среди статусов пользователей. " +
                            "Возможно, стоит проверить среди общих статусов," +
                            " а не среди кастомных либо статус некорректен. " +
                            $"Кастомные статусы были: {JsonConvert.SerializeObject(userStatuses)}" +
                            $". Среди них не было найдено статуса: {statusId}.");
                    }
                    
                    var userStatus = userStatuses.TryGet(statusId);
                    
                    result.Add(new AvailableTaskStatusTransitionOutput
                    {
                        StatusName = userStatus.StatusName,
                        StatusId = statusId,
                        TaskStatusId = ts.TaskStatusId
                    });
                }

                // Иначе из таблицы общих статусов.
                if (!commonStatuses.ContainsKey(statusId))
                {
                    throw new InvalidOperationException(
                        $"Статуса {statusId} нет среди общих статусов. " +
                        "Возможно, стоит проверить среди кастомных статусов пользователей," +
                        " а не среди общих либо статус некорректен. " +
                        $"Общие статусы были: {JsonConvert.SerializeObject(commonStatuses)}" +
                        $". Среди них не было найдено статуса: {statusId}.");
                }
                
                var commonStatuse = commonStatuses.TryGet(statusId);
                    
                result.Add(new AvailableTaskStatusTransitionOutput
                {
                    StatusName = commonStatuse.StatusName,
                    StatusId = statusId,
                    TaskStatusId = ts.TaskStatusId
                });
            }

            // Добавляем текущий статус в доступный переход.
            var currentTaskStatus = await _projectManagmentRepository
                .GetTaskStatusByTaskStatusIdAsync(currentTaskStatusId, templateId);
                
            result.Add(new AvailableTaskStatusTransitionOutput
            {
                StatusName = currentTaskStatus.StatusName,
                StatusId = currentTaskStatus.StatusId,
                TaskStatusId = currentTaskStatus.TaskStatusId
            });

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task ChangeTaskStatusAsync(long projectId, long changeStatusId, long taskId)
    {
        await _projectManagmentRepository.ChangeTaskStatusAsync(projectId, changeStatusId, taskId);
    }

    /// <inheritdoc />
    public async Task UpdateTaskDetailsAsync(long projectId, long taskId, string changedTaskDetails, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectManagmentRepository.UpdateTaskDetailsAsync(projectId, taskId, changedTaskDetails);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateTaskNameAsync(long projectId, long taskId, string changedTaskName, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectManagmentRepository.UpdateTaskNameAsync(projectId, taskId, changedTaskName);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task AttachTaskTagAsync(int tagId, long projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.AttachTaskTagAsync(tagId, projectTaskId, projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task DetachTaskTagAsync(int tagId, long projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.DetachTaskTagAsync(tagId, projectTaskId, projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task CreateTaskLinkDefaultAsync(long taskFromLink, long taskToLink, LinkTypeEnum linkType,
        long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.CreateTaskLinkDefaultAsync(taskFromLink, taskToLink, linkType, projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDefaultAsync(long projectId, long projectTaskId)
    {
        try
        {
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(projectTaskId,
                projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var links = (await _projectManagmentRepository.GetTaskLinksByProjectIdProjectTaskIdAsync(projectId,
                currentTask.TaskId))
                ?.AsList();

            if (links is null || !links.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }

            var linkIds = links.Select(x => x.TaskId);
            
            var tasks = (await _projectManagmentRepository.GetProjectTaskByProjectIdTaskIdsAsync(projectId, linkIds))
                ?.AsList();

            if (tasks is null || !tasks.Any())
            {
                throw new InvalidOperationException(
                    "Не удалось получить связи задачи по Id проекта и Id задачи в рамках проекта." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }
            
            // Текущую задачу не надо выводить в связях текущей задачи.
            var removedCurrentTask = tasks.Find(x => x.ProjectTaskId == projectTaskId);

            if (removedCurrentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу для исключения ее из связей." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            tasks.Remove(removedCurrentTask);

            var result = await ModifyTaskLinkResultAsync(tasks);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод наполняет названия исходя из Id записей.
    /// </summary>
    /// <param name="projectManagmentTaskStatusTemplates">Список статусов, каждый статус может включать
    /// или не включать в себя задачи.</param>
    /// <param name="tasks">Список задач рабочего пространства проекта.</param>
    /// <param name="projectId">Id проекта</param>
    /// <exception cref="InvalidOperationException">Может бахнуть, если какое-то условие не прошли.</exception>
    private async Task ModifyProjectManagmentTaskStatusesResultAsync(
        IEnumerable<ProjectManagmentTaskStatusTemplateOutput> projectManagmentTaskStatusTemplates,
        List<ProjectTaskEntity> tasks, long projectId)
    {
        // Получаем имена авторов задач.
        var authorIds = tasks.Select(x => x.AuthorId);
        var authors = await _userRepository.GetAuthorNamesByAuthorIdsAsync(authorIds);

        if (authors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить авторов задач.");
        }

        var notValidExecutors = tasks.Where(x => x.ExecutorId <= 0);

        // Обязательно логируем такое если обнаружили, но не стопаем выполнение логики.
        if (notValidExecutors.Any())
        {
            var ex = new InvalidOperationException(
                "Найдены невалидные исполнители задач." +
                $" ExecutorIds: {JsonConvert.SerializeObject(notValidExecutors)}." +
                $" ProjectId: {projectId}");

            _logger.LogError(ex, ex.Message);

            // Отправляем ивент в пачку.
            await _pachcaService.SendNotificationErrorAsync(ex);
        }

        // Получаем имена исполнителей задач.
        var executorIds = tasks.Where(x => x.ExecutorId > 0).Select(x => x.ExecutorId);
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей задач.");
        }

        IDictionary<int, ProjectTagOutput> tags = null;

        // Если есть теги, то пойдем получать.
        if (tasks.Any(x => x.TagIds is not null))
        {
            var tagIds = tasks.Where(x => x.TagIds is not null).SelectMany(x => x.TagIds).Distinct();
            tags = await _projectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);
        }

        var typeIds = tasks.Select(x => x.TaskTypeId);
        var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(typeIds);

        var statusIds = tasks.Select(x => x.TaskStatusId);
        var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
            .ToList();

        var resolutionIds = tasks
            .Where(x => x.ResolutionId is not null)
            .Select(x => (int)x.ResolutionId)
            .ToList();

        IDictionary<int, TaskResolutionOutput> resolutions = null;

        // Если есть резолюции задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (resolutionIds.Any())
        {
            resolutions = await _projectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
                resolutionIds);
        }
        
        var priorityIds = tasks
            .Where(x => x.PriorityId is not null)
            .Select(x => (int)x.PriorityId)
            .ToList();
        
        IDictionary<int, TaskPriorityOutput> priorities = null;
        
        // Если есть приоритеты задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (priorityIds.Any())
        {
            priorities = await _projectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                priorityIds);
        }
        
        var watcherIds = tasks
            .Where(x => x.WatcherIds is not null)
            .SelectMany(x => x.WatcherIds)
            .ToList();

        IDictionary<long, UserInfoOutput> watchers = null;

        // Если есть наблюдатели, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (watcherIds.Any())
        {
            watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);
        }

        // Распределяем задачи по статусам.
        foreach (var ps in projectManagmentTaskStatusTemplates)
        {
            var tasksByStatus = tasks.Where(s => s.TaskStatusId == ps.TaskStatusId);

            // Для этого статуса нет задач, пропускаем.
            if (!tasksByStatus.Any())
            {
                continue;
            }

            var mapTasks = _mapper.Map<List<ProjectManagmentTaskOutput>>(tasksByStatus);

            // Добавляем задачи статуса, если есть что добавлять.
            if (mapTasks.Any())
            {
                // Записываем названия исходя от Id полей.
                foreach (var ts in mapTasks)
                {
                    // ФИО автора задачи.
                    ts.AuthorName = authors.TryGet(ts.AuthorId)?.FullName;

                    var executorId = ts.ExecutorId;

                    if (executorId <= 0)
                    {
                        throw new InvalidOperationException("Невалидный исполнитель задачи." +
                                                            $" ExecutorId: {executorId}");
                    }

                    // ФИО исполнителя задачи (если назначен).
                    ts.ExecutorName = executors.TryGet(ts.ExecutorId)?.FullName;
                    
                    var taskStatusName = statuseNames.Find(x => x.StatusId == ts.TaskStatusId)?.StatusName;

                    if (string.IsNullOrEmpty(taskStatusName))
                    {
                        throw new InvalidOperationException("Не удалось получить TaskStatusName: {ts.TaskStatusId}.");
                    }

                    // Название статуса.
                    ts.TaskStatusName = taskStatusName;

                    // Название типа задачи.
                    ts.TaskTypeName = types.TryGet(ts.TaskTypeId)?.TypeName;

                    // Название тегов (меток) задачи.
                    if (tags is not null && tags.Count > 0)
                    {
                        foreach (var tg in ts.TagIds)
                        {
                            ts.TagNames = new List<string> { tags.TryGet(tg)?.TagName };
                        }
                    }

                    // Название резолюции задачи.
                    if (resolutions is not null && resolutions.Count > 0)
                    {
                        var resolutionName = resolutions.TryGet(ts.ResolutionId);

                        // Если резолюции нет, не страшно. Значит не указана у задачи.
                        if (resolutionName is not null)
                        {
                            ts.ResolutionName = resolutions.TryGet(ts.ResolutionId)?.ResolutionName;
                        }
                    }

                    // Названия наблюдателей задачи.
                    if (watchers is not null && watchers.Count > 0)
                    {
                        foreach (var w in ts.WatcherIds)
                        {
                            ts.WatcherNames = new List<string> { watchers.TryGet(w)?.FullName };
                        }
                    }
                    
                    // Название приорита задачи.
                    if (priorities is not null && priorities.Count > 0)
                    {
                        var priorityName = priorities.TryGet(ts.PriorityId);

                        // Если приоритета нет, не страшно. Значит не указана у задачи.
                        if (priorityName is not null)
                        {
                            ts.PriorityName = priorities.TryGet(ts.PriorityId)?.PriorityName;
                        }
                    }
                }

                ps.ProjectManagmentTasks = new List<ProjectManagmentTaskOutput>(mapTasks.Count);
                ps.ProjectManagmentTasks.AddRange(mapTasks);
                
                // Кол-во задач в статусе.
                ps.Total = ps.ProjectManagmentTasks.Count;
            }
        }
    }

    /// <summary>
    /// Метод модифицирует результаты связей задачи.
    /// </summary>
    /// <param name="links">Связи задач.</param>
    /// <returns>Модифицированные данные связей.</returns>
    private async Task<IEnumerable<GetTaskLinkOutput>> ModifyTaskLinkResultAsync(List<ProjectTaskEntity> tasks)
    {
        var result = new List<GetTaskLinkOutput>();
        
        // Получаем имена исполнителей задач.
        var executorIds = tasks.Where(x => x.ExecutorId > 0).Select(x => x.ExecutorId);
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей задач.");
        }
        
        var statusIds = tasks.Select(x => x.TaskStatusId);
        var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
            .ToList();

        // Наполняем выходные данные задачи.
        foreach (var t in tasks)
        {
            var link = new GetTaskLinkOutput
            {
                ExecutorName = executors.TryGet(t.ExecutorId).FullName,
                TaskName = t.Name,
                TaskCode = null, /// TODO: Пока не используется. Вернуться, когда будет реализован вывод названия проекта.
                TaskStatusName = statuseNames.Find(x => x.StatusId == t.TaskStatusId)?.StatusName,
                LastUpdated = t.Updated.ToString("f"), // Например, 17 июля 2015 г. 17:04
                ProjectTaskId = t.ProjectTaskId
            };

            result.Add(link);
        }

        return result;
    }

    #endregion
}