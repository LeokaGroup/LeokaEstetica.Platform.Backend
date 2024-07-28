using Dapper;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Строитель эпика.
/// </summary>
internal class EpicBuilder : AgileObjectBuilder
{
    private const int EPIC_TYPE_ID = 4;
    
    /// <inheritdoc />
    public override async Task InitObjectAsync()
    {
        var task = await BuilderData.ProjectManagmentRepository.GetEpicDetailsByEpicIdAsync(BuilderData.ProjectTaskId,
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
            throw new InvalidOperationException("Не удалось получить автора эпика.");
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
                "Найден невалидный исполнитель эпика." +
                $" ExecutorIds: {JsonConvert.SerializeObject(executorId)}.");

            // Отправляем ивент в пачку.
            await BuilderData.DiscordService.SendNotificationErrorAsync(ex);
        }
        
        // Получаем имена исполнителя эпика.
        var executors = await BuilderData.UserRepository.GetExecutorNamesByExecutorIdsAsync(new[] { executorId });

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей эпика.");
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

    // TODO: Дублируется.
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
        var types = await BuilderData.ProjectManagmentRepository.GetTypeNamesByTypeIdsAsync(new[] { EPIC_TYPE_ID });
        var type = types.TryGet(EPIC_TYPE_ID);
        
        ProjectManagmentTask.TaskTypeName = type.TypeName;
        ProjectManagmentTask.TaskTypeId = type.TypeId;
    }

    /// <inheritdoc />
    public override async Task FillTaskStatusNameAsync()
    {
        var taskStatusId = ProjectManagmentTask.TaskStatusId;
        var statuseName = await BuilderData.ProjectManagmentRepository.GetEpicStatusNameByEpicStatusIdAsync(
            Convert.ToInt32(taskStatusId));

        if (string.IsNullOrEmpty(statuseName))
        {
            throw new InvalidOperationException($"Не удалось получить TaskStatusName: {taskStatusId}.");
        }

        ProjectManagmentTask.TaskStatusName = statuseName;
    }

    /// <inheritdoc />
    public override Task FillResolutionNameAsync()
    {
        throw new NotImplementedException("Эпики не поддерживают резолюции.");
    }

    // TODO: Дублируется.
    /// <inheritdoc />
    public override async Task FillPriorityNameAsync()
    {
        var priorityId = ProjectManagmentTask.PriorityId;

        // Если есть приоритеты эпиков, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у эпиков.
        if (priorityId > 0)
        {
            var priorities = await BuilderData.ProjectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                new[] { priorityId });

            if (priorities is not null && priorities.Count > 0)
            {
                var priorityName = priorities.TryGet(priorityId);

                // Если приоритета нет, не страшно. Значит не указана у эпика.
                if (priorityName is not null)
                {
                    ProjectManagmentTask.PriorityName = priorities.TryGet(priorityId).PriorityName;
                }
            }
        }
    }

    /// <inheritdoc />
    public override Task FillEpicIdAndEpicNameAsync()
    {
        throw new NotImplementedException("Функционал эпикам не нужен.");
    }

    /// <inheritdoc />
    public override Task FillSprintIdAndSprintNameAsync()
    {
        throw new NotImplementedException("Функционал эпикам не нужен.");
    }
    
    /// <inheritdoc />
    public override async Task FillEpicTasksAsync()
    {
        // Получаем настройки проекта.
        var projectSettings = await BuilderData.ProjectSettingsConfigRepository
            .GetProjectSpaceSettingsByProjectIdAsync(BuilderData.ProjectId);
        var projectSettingsItems = projectSettings?.AsList();

        if (projectSettingsItems is null
            || !projectSettingsItems.Any()
            || projectSettingsItems.Any(x => x is null))
        {
            throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                $"ProjectId: {BuilderData.ProjectId}.");
        }

        var template = projectSettingsItems.Find(x =>
            x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
        var templateId = Convert.ToInt32(template!.ParamValue);

        var epicId = await BuilderData.ProjectManagmentRepository.GetEpicIdByProjectEpicIdAsync(BuilderData.ProjectId,
            BuilderData.ProjectTaskId);

        var epicTasks = await BuilderData.ProjectManagmentRepository.GetEpicTasksAsync(BuilderData.ProjectId, epicId,
            templateId);

        if (epicTasks.EpicTasks.Any())
        {
            ProjectManagmentTask.EpicTasks ??= new List<ProjectManagmentTaskOutput>();
            ProjectManagmentTask.EpicTasks.AsList()
                .AddRange(BuilderData.Mapper.Map<IEnumerable<ProjectManagmentTaskOutput>>(epicTasks.EpicTasks));
            
            // Заполняем название исполнителей задач эпика.
            // Обязательно логируем такое если обнаружили, но не стопаем выполнение логики.
            if (ProjectManagmentTask.EpicTasks.Any(x => x.ExecutorId <= 0))
            {
                throw new InvalidOperationException(
                    "Найдены невалидные исполнители задачи эпика." +
                    $" EpicTasks: {JsonConvert.SerializeObject(ProjectManagmentTask.EpicTasks)}.");
            }
            
            // Получаем имена исполнителей задач.
            var executors = await BuilderData.UserRepository.GetExecutorNamesByExecutorIdsAsync(
                    ProjectManagmentTask.EpicTasks.Select(x => x.ExecutorId));

            if (executors.Count == 0)
            {
                throw new InvalidOperationException("Не удалось получить исполнителей задач эпика.");
            }

            foreach (var et in ProjectManagmentTask.EpicTasks)
            {
                et.Executor ??= new Executor();
                et.Executor.ExecutorName = executors.TryGet(et.ExecutorId)?.FullName;
            }
        }
    }
}