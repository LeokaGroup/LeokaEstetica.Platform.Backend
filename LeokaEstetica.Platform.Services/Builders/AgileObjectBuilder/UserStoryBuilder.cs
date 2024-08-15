using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Строитель истории.
/// </summary>
internal class UserStoryBuilder : BaseAgileObjectBuilder
{
    private const int STORY_TYPE_ID = 2;
    
    /// <inheritdoc />
    public override async Task InitObjectAsync()
    {
        var task = await BuilderData.ProjectManagmentRepository.GetUserStoryDetailsByUserStoryIdAsync(
            BuilderData.ProjectTaskId, BuilderData.ProjectId);
        
        ProjectManagmentTask = BuilderData.Mapper.Map<ProjectManagmentTaskOutput>(task);
        ProjectManagmentTask.Executor = new Executor();
    }

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillAuthorNameAsync()
    {
        // Получаем имя автора истории.
        var authors = await BuilderData.UserRepository.GetAuthorNamesByAuthorIdsAsync(
            new[] { ProjectManagmentTask.AuthorId });

        if (authors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить автора истории.");
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
                "Найден невалидный исполнитель истории." +
                $" ExecutorIds: {JsonConvert.SerializeObject(executorId)}.");

            // Отправляем ивент в пачку.
            await BuilderData.DiscordService.SendNotificationErrorAsync(ex);
        }
        
        // Получаем имена исполнителя эпика.
        var executors = await BuilderData.UserRepository.GetExecutorNamesByExecutorIdsAsync(new[] { executorId });

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей истории.");
        }
        
        ProjectManagmentTask.Executor.ExecutorName = executors.TryGet(executors.First().Key).FullName;
    }

    // TODO: Дублируется.
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

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillWatcherNamesAsync()
    {
        var watcherIds = ProjectManagmentTask.WatcherIds;
        
        // Если есть наблюдатели, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у истории.
        if (watcherIds is not null && watcherIds.Any())
        {
            var watchers = await BuilderData.UserRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);

            // Наблюдатели истории.
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

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillTagIdsAsync()
    {
        var tagIds = ProjectManagmentTask.TagIds;
        
        // Если есть теги, то пойдем получать.
        if (tagIds is not null && tagIds.Any())
        {
            var tags = await BuilderData.ProjectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);

            // Название тегов (меток) истории.
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
        var types = await BuilderData.ProjectManagmentRepository.GetTypeNamesByTypeIdsAsync(new[] { STORY_TYPE_ID });
        var type = types.TryGet(STORY_TYPE_ID);
        
        ProjectManagmentTask.TaskTypeName = type.TypeName;
        ProjectManagmentTask.TaskTypeId = type.TypeId;
    }

    /// <inheritdoc />
    public override async Task FillTaskStatusNameAsync()
    {
        var taskStatusId = ProjectManagmentTask.TaskStatusId;
        var statuseName = await BuilderData.ProjectManagmentRepository.GetUserStoryStatusNameByStoryStatusIdAsync(
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
        
        // Если есть резолюции истории, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у истории.
        if (resolutionId > 0)
        {
            var resolutions = await BuilderData.ProjectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
                new[] { resolutionId });

            // Получаем резолюцию истории, если она есть.
            if (resolutions is not null && resolutions.Count > 0)
            {
                ProjectManagmentTask.ResolutionName = resolutions.TryGet(resolutionId).ResolutionName;
            }
        }
    }

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillPriorityNameAsync()
    {
        var priorityId = ProjectManagmentTask.PriorityId;

        // Если есть приоритеты истории, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у истории.
        if (priorityId > 0)
        {
            var priorities = await BuilderData.ProjectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                new[] { priorityId });

            if (priorities is not null && priorities.Count > 0)
            {
                var priorityName = priorities.TryGet(priorityId);

                // Если приоритета нет, не страшно. Значит не указана у истории.
                if (priorityName is not null)
                {
                    ProjectManagmentTask.PriorityName = priorities.TryGet(priorityId).PriorityName;
                }
            }
        }
    }

    // TODO: Функционал еще не сделан.
    /// <inheritdoc />
    public override Task FillEpicIdAndEpicNameAsync()
    {
        throw new NotImplementedException();
    }

    // TODO: Функционал еще не сделан.
    /// <inheritdoc />
    public override Task FillSprintIdAndSprintNameAsync()
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public override Task FillEpicTasksAsync()
    {
        throw new NotImplementedException("Функционал историям не нужен.");
    }
}