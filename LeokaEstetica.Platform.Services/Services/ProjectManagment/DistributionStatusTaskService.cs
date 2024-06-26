﻿using AutoMapper;
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
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

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
    public async Task DistributionStatusTaskAsync(
        IEnumerable<ProjectManagmentTaskStatusTemplateOutput> projectManagmentTaskStatusTemplates,
        List<ProjectTaskExtendedEntity> tasks, ModifyTaskStatuseTypeEnum modifyTaskStatuseType, long projectId,
        int? paginatorStatusId, string strategy, int page = 1)
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
                .Where(x => !excludedStatuses.Contains(x.TaskStatusId));
                    
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

            // Отправляем ивент в пачку.
            await _discordService.SendNotificationErrorAsync(ex);
        }

        // Получаем имена исполнителей задач.
        var executorIds = tasks.Where(x => x.ExecutorId > 0).Select(x => x.ExecutorId);
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей задач.");
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

        var statusIds = tasks.Select(x => x.TaskStatusId);
        var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
            .AsList();

        var resolutionIds = tasks
            .Where(x => x.ResolutionId is not null)
            .Select(x => (int)x.ResolutionId)
            .AsList();

        IDictionary<int, TaskResolutionOutput>? resolutions = null;

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
            .AsList();
        
        IDictionary<int, TaskPriorityOutput>? priorities = null;
        
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
            .AsList();

        IDictionary<long, UserInfoOutput>? watchers = null;

        // Если есть наблюдатели, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (watcherIds.Any())
        {
            watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);
        }
        
        foreach (var ps in projectManagmentTaskStatusTemplates)
        {
            var tasksByStatus = tasks
                .Where(s => s.TaskStatusId == ps.TaskStatusId)
                .OrderByDescending(o => o.Created); // Новые задачи статуса будут выше.

            // Для этого статуса нет задач, пропускаем.
            if (!tasksByStatus.Any())
            {
                continue;
            }

            var mapTasks = _mapper.Map<List<ProjectManagmentTaskOutput>>(tasksByStatus);

            // Добавляем задачи статуса, если есть что добавлять.
            if (mapTasks.Any())
            {
                var userEmails = await _userRepository.GetUserEmailByUserIdsAsync(mapTasks.Select(x => x.ExecutorId));
                var avatarFiles = await _userService.GetUserAvatarFilesAsync(projectId, userEmails);
                
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
                    var executorName = executors.TryGet(ts.ExecutorId)?.FullName;
                    ts.Executor = new Executor
                    {
                        ExecutorName = executorName
                    };

                    if (!avatarFiles.ContainsKey(ts.ExecutorId))
                    {
                        // Получаем дефолтное изображение.
                        ts.Executor.Avatar = avatarFiles.TryGet(0);
                    }

                    else
                    {
                        ts.Executor.Avatar = avatarFiles.TryGet(ts.ExecutorId);
                    }

                    var taskStatusName = statuseNames.Find(x => x.StatusId == ts.TaskStatusId)?.StatusName;

                    if (string.IsNullOrEmpty(taskStatusName))
                    {
                        throw new InvalidOperationException($"Не удалось получить TaskStatusName: {ts.TaskStatusId}.");
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
                                var ex = new InvalidOperationException("Обнаружен наблюдатель с NULL. " +
                                                                       $"WatcherId: {w}");
                                await _discordService.SendNotificationErrorAsync(ex);
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
                
                var statusTasksCount = mapTasks.Count; // Всего задач у статуса.
                var isAppended = false;

                // Действия с задачами, если стратегия представления Scrum.
                if (strategy.Equals("sm"))
                {
                    if (mapTasks.Count > 10)
                    {
                        // При нажатии на кнопку "Показать больше", докидываем в список задач определенного
                        // статуса следующие 10.
                        if (paginatorStatusId.HasValue && paginatorStatusId == ps.TaskStatusId)
                        {
                            ps.ProjectManagmentTasks = new List<ProjectManagmentTaskOutput>(statusTasksCount);
                            
                            // TODO: Надо переделать на IQueryable, чтобы не работать со всем массивом данных.
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
                            // TODO: Надо переделать на IQueryable, чтобы не работать со всем массивом данных.
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
                    // Добавляем задачи статуса.
                    ps.ProjectManagmentTasks = new List<ProjectManagmentTaskOutput>(statusTasksCount);
                    ps.ProjectManagmentTasks.AddRange(mapTasks);
                }
                
                // Сортируем задачи статуса по убываниию даты создания. Новые задачи будут выше.
                ps.ProjectManagmentTasks = ps.ProjectManagmentTasks.OrderByDescending(o => o.Created).AsList();

                // Кол-во задач в статусе.
                ps.Total = statusTasksCount;
            }
        }
    }
}