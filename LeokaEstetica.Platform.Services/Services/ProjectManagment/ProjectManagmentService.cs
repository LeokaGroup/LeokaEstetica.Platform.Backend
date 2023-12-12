using System.Diagnostics;
using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Abstractions.Services.Pachca;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Factors;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRepository">Репозиторий управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="mapper">Репозиторий пользователей.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="pachcaService">Сервис уведомлений пачки.</param>
    /// </summary>
    public ProjectManagmentService(ILogger<ProjectManagmentService> logger,
        IProjectManagmentRepository projectManagmentRepository,
        IMapper mapper,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IPachcaService pachcaService)
    {
        _logger = logger;
        _projectManagmentRepository = projectManagmentRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _pachcaService = pachcaService;
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
                    if (!item.HasItems)
                    {
                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName
                        });
                    }

                    else
                    {
                        ProjectManagmentHeaderSettings mapItems;

                        try
                        {
                            mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderSettings>(item.Items);
                        }

                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Ошибка при десериализации элементов меню настроек." +
                                                  $" Settings: {item.Items}");
                            throw;
                        }

                        if (mapItems is not null)
                        {
                            var settings = mapItems.Items.OrderBy(o => o.Position);

                            var selectedSettings = settings.Select(x => new ProjectManagmentHeader
                            {
                                Label = x.ItemName,
                                Id = ProjectManagmentDestinationTypeEnum.Settings.ToString()
                            });

                            result.AddRange(selectedSettings.Select(x => new ProjectManagmentHeaderResult
                            {
                                Label = x.Label,
                                Id = x.Id
                            }));
                        }
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
    public async Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync(
        long? templateId)
    {
        try
        {
            var result = await _projectManagmentRepository.GetProjectManagmentTemplatesAsync(templateId);

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

            var statusesDict = await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(
                templateStatusIds);

            foreach (var ts in items)
            {
                var statusId = ts.StatusId;

                // Если не нашли такогго статуса в таблице маппинга многие-ко-многим.
                if (!statusesDict.ContainsKey(statusId))
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {statusId}");
                }

                ts.TemplateId = statusesDict.TryGet(statusId);
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
    /// <param name="strategy">Выбранная стратегия представления.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    public async Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(long projectId,
        string strategy, int templateId, string account)
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

            // Получаем набор статусов, которые входят в выбранный шаблон.
            var items = await GetProjectManagmentTemplatesAsync(templateId);
            var templateStatusesItems = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
            var statuses = templateStatusesItems?.ToList();

            if (statuses is null || !statuses.Any())
            {
                throw new InvalidOperationException("Не удалось получить набор статусов шаблона." +
                                                    $" TemplateId: {templateId}." +
                                                    $"ProjectId: {projectId}." +
                                                    $"Strategy: {strategy}.");
            }

            // Проставляем Id шаблона статусам.
            await SetProjectManagmentTemplateIdsAsync(statuses);

            var result = new ProjectManagmentWorkspaceResult
            {
                ProjectManagmentTaskStatuses = statuses.First().ProjectManagmentTaskStatusTemplates
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

                // Наполняем названиями исхродя из Id полей.
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

            var result = await ModifyProjectManagmentTaskDetailsResultAsync(task);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task CreateProjectTaskAsync(CreateProjectManagementTaskInput projectManagementTaskInput,
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

            var projectId = projectManagementTaskInput.ProjectId;
            
            // Находим наибольший Id задачи в рамках проекта и увеличиваем его.
            var maxProjectTaskId = await _projectManagmentRepository.GetLastProjectTaskIdAsync(projectId);

            if (maxProjectTaskId <= 0)
            {
                throw new InvalidOperationException("Не удалось получить наибольший Id задачи в рамках проекта." +
                                                    $"ProjectId: {projectId}");
            }

            ProjectTaskEntity addedProjectTask;
            
            // Ускоренное создание задачи.
            if (projectManagementTaskInput.IsQuickCreate)
            {
                addedProjectTask = CreateProjectTaskFactory.CreateQuickProjectTask(projectManagementTaskInput, userId);
            }

            // Обычное создание задачи.
            else
            {
                // При обычном создании задачи требуется валидация всех параметров.
                var isValidParameters = IsValidCreateProjectTaskParameters(projectManagementTaskInput);

                // Сюда заходить не должно никогда. Если это произошло, то следовать логу.
                if (!isValidParameters)
                {
                    throw new InvalidOperationException(
                        "Ошибка при создании задачи." +
                        " Валидация не покрыла исключение, нужно изучить логи и добавить обработку ситуации.");
                }
                
                // Если не был указан исполнитель задачи, то исполнителем задачи будет ее автор.
                if (!projectManagementTaskInput.ExecutorId.HasValue)
                {
                    projectManagementTaskInput.ExecutorId = userId!;
                }
                
                addedProjectTask = CreateProjectTaskFactory.CreateProjectTask(projectManagementTaskInput, userId);
            }

            addedProjectTask.ProjectTaskId = ++maxProjectTaskId;
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

        IDictionary<int, string> tags = null;

        // Если есть теги, то пойдем получать.
        if (tasks.Any(x => x.TagIds is not null))
        {
            var tagIds = tasks.SelectMany(x => x.TagIds);
            tags = await _projectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);
        }

        var typeIds = tasks.Select(x => x.TaskTypeId);
        var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(typeIds);

        var statusIds = tasks.Select(x => x.TaskStatusId);
        var statuseNames = await _projectManagmentRepository.GetStatusNamesByStatusIdsAsync(statusIds);

        var resolutionIds = tasks
            .Where(x => x.ResolutionId is not null)
            .Select(x => (int)x.ResolutionId)
            .ToList();

        IDictionary<int, string> resolutions = null;

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
        
        IDictionary<int, string> priorities = null;
        
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

                    // Название статуса.
                    ts.TaskStatusName = statuseNames.TryGet(ts.TaskStatusId);

                    // Название типа задачи.
                    ts.TaskTypeName = types.TryGet(ts.TaskTypeId);

                    // Название тегов (меток) задачи.
                    if (tags is not null && tags.Count > 0)
                    {
                        foreach (var tg in ts.TagIds)
                        {
                            ts.TagNames = new List<string> { tags.TryGet(tg) };
                        }
                    }

                    // Название резолюции задачи.
                    if (resolutions is not null && resolutions.Count > 0)
                    {
                        var resolutionName = resolutions.TryGet(ts.ResolutionId);

                        // Если резолюции нет, не страшно. Значит не указана у задачи.
                        if (resolutionName is not null)
                        {
                            ts.ResolutionName = resolutions.TryGet(ts.ResolutionId);
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
                            ts.PriorityName = priorities.TryGet(ts.PriorityId);
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
    /// Метод наполняет задачу названиями по Id полей.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Задача модифицируемая наполненными полями.</returns>
    private async Task<ProjectManagmentTaskOutput> ModifyProjectManagmentTaskDetailsResultAsync(ProjectTaskEntity task)
    {
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
                    tagNames.Add(tags.TryGet(tg));
                }
            
                result.TagNames = tagNames;
            }
        }

        var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(new[] { task.TaskTypeId });
        result.TaskTypeName = types.TryGet(result.TaskTypeId);
        
        var statuseNames = await _projectManagmentRepository.GetStatusNamesByStatusIdsAsync(
            new[] { task.TaskStatusId });
        
        result.TaskStatusName = statuseNames.TryGet(result.TaskStatusId);

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
                result.ResolutionName = resolutions.TryGet(result.ResolutionId);
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
                    result.PriorityName = priorities.TryGet(priorityId.Value);
                }
            }
        }

        return result;
    }

    #endregion

    /// <summary>
    /// TODO: Добавить отправку на фронт ивент на каждое сообщение через сокеты.
    /// Метод валидирует параметры перед созданием задачи.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <returns>Признак результата проверки.</returns>
    private bool IsValidCreateProjectTaskParameters(CreateProjectManagementTaskInput projectManagementTaskInput)
    {
        var exceptions = new List<InvalidOperationException>();
        
        if (!projectManagementTaskInput.TaskStatusId.HasValue)
        {
            exceptions.Add(new InvalidOperationException(
                $"Статус задачи не указан. Данные задачи: {JsonConvert.SerializeObject(projectManagementTaskInput)}"));
        }

        if (!projectManagementTaskInput.TaskTypeId.HasValue)
        {
            exceptions.Add(new InvalidOperationException(
                $"Тип задачи не указан. Данные задачи: {JsonConvert.SerializeObject(projectManagementTaskInput)}"));
        }

        var ex = new AggregateException("Ошибка при создании задачи.", exceptions);
        _logger.LogCritical(ex, ex.Message);
        throw ex;
    }
}