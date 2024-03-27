using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Строитель задачи.
/// </summary>
public class TaskBuilder : AgileObjectBuilder
{
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPachcaService _pachcaService;
    private readonly IUserService _userService;
    private readonly IProjectManagmentTemplateRepository _projectManagmentTemplateRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов модуля УП.</param>
    /// <param name="mapper">Маппер.</param>
    public TaskBuilder(IProjectManagmentRepository projectManagmentRepository,
        IUserRepository userRepository,
        IPachcaService pachcaService,
        IUserService userService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        IMapper mapper)
        : base(projectManagmentRepository,
            userRepository,
            pachcaService,
            userService,
            projectManagmentTemplateRepository,
            mapper)
    {
        _projectManagmentRepository = projectManagmentRepository;
        _userRepository = userRepository;
        _pachcaService = pachcaService;
        _userService = userService;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
    }

    /// <inheritdoc />
    public override async Task InitObjectAsync(long projectTaskId, long projectId)
    {
        var task = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(ProjectTaskId, ProjectId);
        
        ProjectManagmentTask = Mapper.Map<ProjectManagmentTaskOutput>(task);
        
        ProjectManagmentTask.Executor = new Executor();
    }

    /// <inheritdoc />
    public override async Task FillAuthorNameAsync()
    {
        // Получаем имя автора задачи.
        var authors = await _userRepository.GetAuthorNamesByAuthorIdsAsync(new[] { ProjectManagmentTask.AuthorId });

        if (authors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить автора задачи.");
        }

        ProjectManagmentTask.AuthorName = authors.TryGet(authors.First().Key).FullName;
    }

    /// <inheritdoc />
    public override async Task FillExecutorNameAsync()
    {
        var executorId = ProjectManagmentTask.ExecutorId;
        
        // Обязательно логируем такое если обнаружили, но не стопаем выполнение логики.
        if (executorId <= 0)
        {
            var ex = new InvalidOperationException(
                "Найден невалидный исполнитель задачи." +
                $" ExecutorIds: {JsonConvert.SerializeObject(executorId)}.");

            // Отправляем ивент в пачку.
            await _pachcaService.SendNotificationErrorAsync(ex);
        }
        
        // Получаем имена исполнителя задачи.
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(new[] { executorId });

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей задач.");
        }
        
        ProjectManagmentTask.Executor.ExecutorName = executors.TryGet(executors.First().Key).FullName;
    }

    /// <inheritdoc />
    public override async Task FillExecutorAvatarAsync()
    {
        var executorId = ProjectManagmentTask.ExecutorId;
        var userEmails = await _userRepository.GetUserEmailByUserIdsAsync(new List<long> { executorId });
        var avatarFiles = await _userService.GetUserAvatarFilesAsync(ProjectId, userEmails);
        
        if (!avatarFiles.ContainsKey(executorId))
        {
            // Получаем дефолтное изображение.
            ProjectManagmentTask.Executor.Avatar = avatarFiles.TryGet(0);
        }

        else
        {
            ProjectManagmentTask.Executor.Avatar = avatarFiles.TryGet(executorId);
        }
    }

    /// <inheritdoc />
    public override async Task FillWatcherNamesAsync()
    {
        var watcherIds = ProjectManagmentTask.WatcherIds;
        
        // Если есть наблюдатели, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (watcherIds is not null && watcherIds.Any())
        {
            var watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);

            // Наблюдатели задачи.
            if (watchers is not null && watchers.Count > 0)
            {
                var watcherNames = new List<string>();

                foreach (var w in watcherIds)
                {
                    var watcher = watchers.TryGet(w);

                    if (!string.IsNullOrWhiteSpace(watcher?.FullName))
                    {
                        watcherNames.Add(watcher.FullName);
                    }

                    else
                    {
                        watcherNames.Add(watcher!.Email);
                    }
                }

                ProjectManagmentTask.WatcherNames = watcherNames;
            }
        }
    }

    /// <inheritdoc />
    public override async Task FillTagIdsAsync()
    {
        var tagIds = ProjectManagmentTask.TagIds;
        
        // Если есть теги, то пойдем получать.
        if (tagIds is not null && tagIds.Any())
        {
            var tags = await _projectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);

            // Название тегов (меток) задачи.
            if (tags is not null && tags.Count > 0)
            {
                var tagNames = new List<string>();

                foreach (var tg in tagIds)
                {
                    var tgName = tags.TryGet(tg).TagName;
                    tagNames.Add(tgName);
                }

                ProjectManagmentTask.TagNames = tagNames;
            }
        }
    }

    /// <inheritdoc />
    public override async Task FillTaskTypeNameAsync()
    {
        var taskTypeId = ProjectManagmentTask.TaskTypeId;
        var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(new[] { taskTypeId });
        ProjectManagmentTask.TaskTypeName = types.TryGet(taskTypeId).TypeName;
    }

    /// <inheritdoc />
    public override async Task FillTaskStatusNameAsync()
    {
        var taskStatusId = ProjectManagmentTask.TaskStatusId;
        var statuseName = await _projectManagmentTemplateRepository.GetStatusNameByTaskStatusIdAsync(
            Convert.ToInt32(taskStatusId));

        if (string.IsNullOrEmpty(statuseName))
        {
            throw new InvalidOperationException($"Не удалось получить TaskStatusName: {taskStatusId}.");
        }

        ProjectManagmentTask.TaskStatusName = statuseName;
    }

    /// <inheritdoc />
    public override async Task FillResolutionNameAsync()
    {
        var resolutionId = ProjectManagmentTask.ResolutionId;
        
        // Если есть резолюции задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (resolutionId > 0)
        {
            var resolutions = await _projectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
                new[] { resolutionId });

            // Получаем резолюцию задачи, если она есть.
            if (resolutions is not null && resolutions.Count > 0)
            {
                ProjectManagmentTask.ResolutionName = resolutions.TryGet(resolutionId).ResolutionName;
            }
        }
    }

    /// <inheritdoc />
    public override async Task FillPriorityNameAsync()
    {
        var priorityId = ProjectManagmentTask.PriorityId;

        // Если есть приоритеты задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (priorityId > 0)
        {
            var priorities = await _projectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                new[] { priorityId });

            if (priorities is not null && priorities.Count > 0)
            {
                var priorityName = priorities.TryGet(priorityId);

                // Если приоритета нет, не страшно. Значит не указана у задачи.
                if (priorityName is not null)
                {
                    ProjectManagmentTask.PriorityName = priorities.TryGet(priorityId).PriorityName;
                }
            }
        }
    }

    /// <inheritdoc />
    public override async Task FillEpicIdAndEpicNameAsync()
    {
        // Получаем эпик, в которую добавлена задача.
        var taskEpic = await _projectManagmentRepository.GetTaskEpicAsync(ProjectId, ProjectTaskId);

        if (taskEpic is not null)
        {
            ProjectManagmentTask.EpicId = taskEpic.EpicId;
            ProjectManagmentTask.EpicName = taskEpic.EpicName;
        }
    }

    /// <inheritdoc />
    public override async Task FillSprintIdAndSprintNameAsync()
    {
        // Получаем спринт, в который входит задача.
        var sprint = await _projectManagmentRepository.GetSprintTaskAsync(ProjectId, ProjectTaskId);

        if (sprint is not null)
        {
            ProjectManagmentTask.SprintId = sprint.SprintId;
            ProjectManagmentTask.SprintName = sprint.SprintName;
        }
    }
}