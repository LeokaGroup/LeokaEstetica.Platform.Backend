using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// TODO: Код, который дублируется в этом и в других билдерах, подумать, как можно вынести куда то.
/// TODO: Например через паттерн шаблонный метод или еще + паттерн адаптер, так как наследование от нескольких классов невозможно.
/// Строитель задачи.
/// </summary>
internal class TaskBuilder : AgileObjectBuilder
{
    /// <inheritdoc />
    public override async Task InitObjectAsync()
    {
        var task = await BuilderData.ProjectManagmentRepository.GetTaskDetailsByTaskIdAsync(BuilderData.ProjectTaskId,
            BuilderData.ProjectId);
        
        ProjectManagmentTask = BuilderData.Mapper.Map<ProjectManagmentTaskOutput>(task);
        ProjectManagmentTask.Executor = new Executor();
    }

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillAuthorNameAsync()
    {
        // Получаем имя автора задачи.
        var authors = await BuilderData.UserRepository.GetAuthorNamesByAuthorIdsAsync(
            new[] { ProjectManagmentTask.AuthorId });

        if (authors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить автора задачи.");
        }

        ProjectManagmentTask.AuthorName = authors.TryGet(authors.First().Key).FullName;
    }

    // TODO: Дублируется.
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
            await BuilderData.DiscordService.SendNotificationErrorAsync(ex);
        }
        
        // Получаем имена исполнителя задачи.
        var executors = await BuilderData.UserRepository.GetExecutorNamesByExecutorIdsAsync(new[] { executorId });

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
        var userEmails = await BuilderData.UserRepository.GetUserEmailByUserIdsAsync(new List<long> { executorId });
        var avatarFiles = await BuilderData.UserService.GetUserAvatarFilesAsync(BuilderData.ProjectId, userEmails);
        
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
            var watchers = await BuilderData.UserRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);

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
            var tags = await BuilderData.ProjectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);

            // Название тегов (меток) задачи.
            if (tags is not null && tags.Count > 0)
            {
                var tagNames = new List<string>();

                foreach (var tg in tagIds)
                {
                    var tgName = tags.TryGet(tg)?.TagName;
                    if (tgName is not null)
                    {
                        tagNames.Add(tgName);
                    }
                }

                ProjectManagmentTask.TagNames = tagNames;
            }
        }
    }

    /// <inheritdoc />
    public override async Task FillTaskTypeNameAsync()
    {
        var taskTypeId = ProjectManagmentTask.TaskTypeId;
        var types = await BuilderData.ProjectManagmentRepository.GetTypeNamesByTypeIdsAsync(new[] { taskTypeId });
        ProjectManagmentTask.TaskTypeName = types.TryGet(taskTypeId).TypeName;
    }

    /// <inheritdoc />
    public override async Task FillTaskStatusNameAsync()
    {
        var taskStatusId = ProjectManagmentTask.TaskStatusId;
        var statuseName = await BuilderData.ProjectManagmentTemplateRepository.GetStatusNameByTaskStatusIdAsync(
            Convert.ToInt32(taskStatusId));

        if (string.IsNullOrEmpty(statuseName))
        {
            throw new InvalidOperationException($"Не удалось получить TaskStatusName: {taskStatusId}.");
        }

        ProjectManagmentTask.TaskStatusName = statuseName;
    }

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillResolutionNameAsync()
    {
        var resolutionId = ProjectManagmentTask.ResolutionId;
        
        // Если есть резолюции задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (resolutionId > 0)
        {
            var resolutions = await BuilderData.ProjectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
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
            var priorities = await BuilderData.ProjectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
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
        var taskEpic = await BuilderData.ProjectManagmentRepository.GetTaskEpicAsync(BuilderData.ProjectId,
            BuilderData.ProjectTaskId);

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
        var sprint = await BuilderData.ProjectManagmentRepository.GetSprintTaskAsync(BuilderData.ProjectId,
            BuilderData.ProjectTaskId);

        if (sprint is not null)
        {
            ProjectManagmentTask.SprintId = sprint.SprintId;
            ProjectManagmentTask.SprintName = sprint.SprintName;
        }
    }

    /// <inheritdoc />
    public override Task FillEpicTasksAsync()
    {
        throw new NotImplementedException("Функционал задачам не нужен.");
    }
}