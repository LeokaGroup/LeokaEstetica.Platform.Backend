using System.Diagnostics;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement.Paginator;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

class ProjectManagmentTaskStatusTemplateData
{
    /// <summary>
    /// Задачи конкретного статуса.
    /// </summary>
    public IEnumerable<ProjectTaskExtendedEntity>? TasksByStatus { get; set; }
}

/// <summary>
/// Класс реализует методы сервиса, который распределяет задачи по статусам.
/// </summary>
internal class DistributionStatusTaskService : IDistributionStatusTaskService
{
    /// <summary>
    /// Кол-во задач у статуса. Если применяется Scrum.
    /// </summary>
    private const int _scrumPageSize = 10;

    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IDiscordService _discordService;
    private readonly ILogger<DistributionStatusTaskService> _logger;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly IProjectManagmentTemplateRepository _projectManagmentTemplateRepository;
    private readonly IUserService _userService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов проекто.</param>
    /// <param name="userService">Сервис пользователей.</param> /// 
    public DistributionStatusTaskService(IMapper mapper,
        IUserRepository userRepository,
        IDiscordService discordService,
        ILogger<DistributionStatusTaskService> logger,
        IProjectManagmentRepository projectManagmentRepository,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        IUserService userService)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _discordService = discordService;
        _logger = logger;
        _projectManagmentRepository = projectManagmentRepository;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _userService = userService;
    }

    /// <inheritdoc />
    public async Task<List<ProjectManagmentTaskStatusTemplateOutput>> DistributionStatusTaskAsync(
        List<ProjectManagmentTaskStatusTemplateOutput> projectManagmentTaskStatusTemplates,
        List<ProjectTaskExtendedEntity> tasks,
        ModifyTaskStatuseTypeEnum modifyTaskStatuseType, long projectId, int? paginatorStatusId, string strategy,
        int page = 1)
    {
        // Исключаем некоторые статусы из бэклога, так как из них нельзя спланировать спринт.
        if (modifyTaskStatuseType == ModifyTaskStatuseTypeEnum.Backlog)
        {
            var excludedStatuses = new[]
            {
                (long)ProjectTaskStatusEnum.Completed,
                (long)ProjectTaskStatusEnum.Archive,
                (long)ProjectTaskStatusEnum.Closed
            };

            projectManagmentTaskStatusTemplates = projectManagmentTaskStatusTemplates
                .Where(x => !excludedStatuses.Contains(x.TaskStatusId))
                .AsList();

            tasks = tasks.Where(x => !excludedStatuses.Contains(x.TaskStatusId)).AsList();
        }

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

            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
        }

        // Получаем имена исполнителей задач.
        var executorIds = tasks
            .Where(x => x.ExecutorId > 0)
            .Select(x => x.ExecutorId)
            .Distinct()
            .AsList();

        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

        // Обязательно логируем такое если обнаружили, но не стопаем выполнение логики.
        if (executors.Count == 0)
        {
            var ex = new InvalidOperationException("Не удалось получить исполнителей задач.");

            _logger.LogError(ex, ex.Message);

            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
        }

        IDictionary<int, ProjectTagOutput>? tags = null;

        // Если есть теги, то пойдем получать.
        if (tasks.Any(x => x.TagIds is not null))
        {
            var tagIds = tasks.Where(x => x.TagIds is not null).SelectMany(x => x.TagIds).Distinct();
            tags = await _projectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);
        }

        var typeIds = tasks.Select(x => x.TaskTypeId);
        var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(typeIds);

        var statusIds = tasks
            .Where(x => x.TaskStatusId > 0)
            .Select(x => x.TaskStatusId)
            .Distinct();

        var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
            .AsList();

        // Получаем системные статусы эпиков и историй из БД, если у проекта есть такие.
        var aggregateSystemStatuses = new List<StoryAndEpicSystemStatusOutput>();

        if (tasks.Any(x => new[] { (int)ProjectTaskTypeEnum.Epic, (int)ProjectTaskTypeEnum.Story }
                .Contains(x.TaskTypeId)))
        {
            aggregateSystemStatuses = (await _projectManagmentRepository.GetEpicAndStorySystemStatusesAsync())
                ?.AsList();
        }

        var resolutionIds = tasks
            .Where(x => x.ResolutionId is not null)
            .Select(x => (int)x.ResolutionId)
            .AsList();

        IDictionary<int, TaskResolutionOutput>? resolutions = null;

        // Если есть резолюции задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (resolutionIds.Count > 0)
        {
            resolutions = await _projectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
                resolutionIds);
        }

        var priorityIds = tasks
            .Where(x => x.PriorityId is not null)
            .Select(x => (int)x.PriorityId)
            .AsList();

        IDictionary<int, TaskPriorityOutput>? priorities = null;

        // Если есть приоритеты задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (priorityIds.Count > 0)
        {
            priorities = await _projectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                priorityIds);
        }

        var watcherIds = tasks
            .Where(x => x.WatcherIds is not null)
            .SelectMany(x => x.WatcherIds)
            .AsList();

        IDictionary<long, UserInfoOutput>? watchers = null;

        // Если есть наблюдатели, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (watcherIds.Count > 0)
        {
            watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);
        }

        // TODO: Понадобиться, когда вынесем все параметры в него.
        // var taskStatusTemplateData = new ProjectManagmentTaskStatusTemplateData();

        var epicIds = tasks
            .Where(x => (int)ProjectTaskTypeEnum.Epic == x.TaskTypeId)
            .Select(y => y.TaskId);

        var epicStatusesDict = await _projectManagmentRepository.GetEpicStatusesDictionaryAsync(epicIds);

        var storyIds = tasks
            .Where(x => (int)ProjectTaskTypeEnum.Story == x.TaskTypeId)
            .Select(y => y.TaskId);

        var storyStatusesDict = await _projectManagmentRepository.GetStoryStatusesDictionaryAsync(storyIds);

        var userEmails = await _userRepository.GetUserEmailByUserIdsAsync(executorIds);
        var avatarFilesDict = await _userService.GetUserAvatarFilesAsync(projectId, userEmails);
        
        // Работаем с эпиками и историями.
        // Распределение задач происходит без учета шаблона проекта.
        // Распределяем системные статусы - они не привязаны к шаблонам.
        var tasksWithStoryAndEpic = tasks.Where(x =>
                new[] { (int)ProjectTaskTypeEnum.Epic, (int)ProjectTaskTypeEnum.Story }
                    .Contains(x.TaskTypeId))
            ?.AsList();

        tasks.RemoveAll(x => tasksWithStoryAndEpic.Select(y => y.TaskId).Contains(x.TaskId));

        // Работаем с задачами и ошибками. Распределяем задачи и ошибки по статусам.
        // Распределение задач происходит на основе шаблона проекта.
        // Системные статусы здесь не заполняем и не учитываем.
        foreach (var ps in projectManagmentTaskStatusTemplates)
        {
            var taskByStatus = tasks
                .Where(x => ps.TaskStatusId == x.TaskStatusId)
                .AsList();
                
            // Задач статуса нету, пропускаем.
            if (taskByStatus.Count == 0)
            {
                continue;
            }

            await FillAndDistributionStatusTaskAsync(taskByStatus, page, tags, types, resolutions,
                priorities, watchers, authors, statuseNames, executors, ps,
                strategy, paginatorStatusId, epicStatusesDict, storyStatusesDict, avatarFilesDict);
        }

        if (tasksWithStoryAndEpic is null || tasksWithStoryAndEpic.Count == 0)
        {
            throw new InvalidOperationException("Ошибка получения системных статусов эпика и истории.");
        }

        var mapSystemStatuses = _mapper.Map<List<ProjectManagmentTaskStatusTemplateOutput>>(
            aggregateSystemStatuses);

        // Распределяем эпики и истории по статусам.
        foreach (var systemStatus in mapSystemStatuses)
        {
            var taskByStatus = tasksWithStoryAndEpic
                .Where(x => systemStatus.TaskStatusId == x.TaskStatusId)
                .AsList();

            // Задач статуса нету, пропускаем.
            if (taskByStatus.Count == 0)
            {
                continue;
            }

            await FillAndDistributionStatusTaskAsync(taskByStatus, page, tags, types, resolutions,
                priorities, watchers, authors, statuseNames, executors, systemStatus,
                strategy, paginatorStatusId, epicStatusesDict, storyStatusesDict, avatarFilesDict);
        }

        // Используется вне этого сервиса.
        projectManagmentTaskStatusTemplates = new List<ProjectManagmentTaskStatusTemplateOutput>(
            projectManagmentTaskStatusTemplates.Union(mapSystemStatuses));

        return projectManagmentTaskStatusTemplates;
    }

    // TODO: Вынести в отдельный класс все эти параметры, их тут уже много.
    // TODO: Класс в самом верху уже заведен, в него поместить всех их.
    private async Task FillAndDistributionStatusTaskAsync(List<ProjectTaskExtendedEntity> taskByStatus,
        int page, IDictionary<int, ProjectTagOutput>? tags, IDictionary<int, TaskTypeOutput>? types,
        IDictionary<int, TaskResolutionOutput>? resolutions, IDictionary<int, TaskPriorityOutput>? priorities,
        IDictionary<long, UserInfoOutput>? watchers, IDictionary<long, UserInfoOutput>? authors,
        List<ProjectManagmentTaskStatusTemplateEntity>? statuseNames, IDictionary<long, UserInfoOutput>? executors,
        ProjectManagmentTaskStatusTemplateOutput ps, string strategy, int? paginatorStatusId,
        IDictionary<long, StoryAndEpicSystemStatusOutput> epicStatusesDict,
        IDictionary<long, StoryAndEpicSystemStatusOutput> storyStatusesDict,
        IDictionary<long, FileContentResult>? avatarFilesDict)
    {
        var mapTasks = _mapper.Map<List<ProjectManagmentTaskOutput>>(taskByStatus);

        // Добавляем задачи статуса, если есть что добавлять.
        if (mapTasks.Count > 0)
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

                // Заполняем данные исполнителя задачи (если назначен).
                var executorName = executors.TryGet(ts.ExecutorId)?.FullName ?? string.Empty;

                ts.Executor = new Executor
                {
                    ExecutorName = executorName.Trim()
                };

                if (!avatarFilesDict.ContainsKey(ts.ExecutorId))
                {
                    // Получаем дефолтное изображение.
                    ts.Executor.Avatar = avatarFilesDict.TryGet(0);
                }

                else
                {
                    ts.Executor.Avatar = avatarFilesDict.TryGet(ts.ExecutorId);
                }

                string? taskStatusName = null;
                
                if (new[] { (long)ProjectTaskTypeEnum.Task, (long)ProjectTaskTypeEnum.Error }
                    .Contains(ts.TaskTypeId))
                {
                    taskStatusName = statuseNames.Find(x => x.StatusId == ts.TaskStatusId)?.StatusName;
                }

                else if (ts.TaskTypeId == (long)ProjectTaskTypeEnum.Story)
                {
                    ts.TaskStatusId = storyStatusesDict.TryGet(ts.TaskId)?.StatusId ??
                                      throw new InvalidOperationException("Ошибка определения статуса истории. " +
                                                                          $"StoryId: {ts.TaskId}.");

                    ts.TaskStatusName = storyStatusesDict.TryGet(ts.TaskId)?.StatusName ??
                                        throw new InvalidOperationException(
                                            "Ошибка определения названия статуса истории. " +
                                            $"StoryId: {ts.TaskId}.");
                }

                else if (ts.TaskTypeId == (long)ProjectTaskTypeEnum.Epic)
                {
                    ts.TaskStatusId = epicStatusesDict.TryGet(ts.TaskId)?.StatusId ??
                                      throw new InvalidOperationException("Ошибка определения статуса эпика. " +
                                                                          $"EpicId: {ts.TaskId}.");

                    ts.TaskStatusName = epicStatusesDict.TryGet(ts.TaskId)?.StatusName ??
                                        throw new InvalidOperationException(
                                            "Ошибка определения названия статуса эпика. " +
                                            $"EpicId: {ts.TaskId}.");
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
                        var watcher = watchers.TryGet(w)?.FullName;

                        // Если такое бахнуло, то не добавляем в список, но и не ломаем приложение.
                        // Просто логируем такое.
                        if (watcher is null)
                        {
                            var ex = new InvalidOperationException("Не найден наблюдатель задачи. " +
                                                                   $"WatcherId: {w}");

                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                            _logger.LogError(ex, ex.Message);

                            continue;
                        }

                        if (ts.WatcherNames is null)
                        {
                            ts.WatcherNames = new List<string>();
                        }

                        ts.WatcherNames.Add(watcher);
                    }
                }

                // Название приоритета задачи.
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

            var statusTasksCount = mapTasks.Count; // Всего задач у статуса.
            var isAppended = false;

            ps.ProjectManagmentTasks ??= new List<ProjectManagmentTaskOutput>(statusTasksCount);

            // Действия с задачами, если стратегия представления Scrum.
            if (strategy.Equals("sm"))
            {
                if (mapTasks.Count > 10)
                {
                    // При нажатии на кнопку "Показать больше", докидываем в список задач определенного
                    // статуса следующие 10.
                    if (paginatorStatusId.HasValue && paginatorStatusId == ps.TaskStatusId)
                    {
                        // Разбиваем пагинатором следующие 10 задач, которые будем докидывать в список.
                        var appendedTasks = mapTasks
                            .Skip((page - 1) * _scrumPageSize)
                            .Take(_scrumPageSize)
                            .AsList();

                        var appendedTaskIds = appendedTasks.Select(x => x.TaskId).AsList();

                        foreach (var t in mapTasks)
                        {
                            // Наполняем уже существующий список определенного статуса.
                            if (!appendedTaskIds.Contains(t.TaskId))
                            {
                                ps.ProjectManagmentTasks.Add(t);
                            }
                        }

                        // Добавляем новые 10 задач к тем, что уже выведены на фронте.
                        ps.ProjectManagmentTasks.AddRange(appendedTasks);
                        isAppended = true;
                    }

                    // Иначе применить пагинатор для всех статусов шаблона.
                    else if (!paginatorStatusId.HasValue)
                    {
                        mapTasks = mapTasks
                            .Skip((page - 1) * _scrumPageSize)
                            .Take(_scrumPageSize)
                            .AsList();
                    }
                }
            }

            // Заполняем модель пагинатора для фронта, чтобы он кидал последующие запросы с параметрами страниц.
            ps.Paginator = new Paginator(statusTasksCount, page, _scrumPageSize);

            // Если задачи уже докидывались, не добавляем снова.
            if (!isAppended)
            {
                // Добавляем задачи статуса, исключая эпик и историю. Для них отдельный список.
                ps.ProjectManagmentTasks.AddRange(mapTasks);

                // Сортируем задачи статуса по убываниию даты создания. Новые задачи будут выше.
                ps.ProjectManagmentTasks = ps.ProjectManagmentTasks.OrderByDescending(o => o.Created).AsList();

                // Кол-во задач в статусе.
                ps.Total = statusTasksCount;
            }
        }
    }
}