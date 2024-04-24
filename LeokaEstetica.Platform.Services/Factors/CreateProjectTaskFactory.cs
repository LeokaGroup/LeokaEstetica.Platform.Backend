using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки для создания задачи проекта.
/// </summary>
internal static class CreateProjectTaskFactory
{
    /// <summary>
    /// Метод создает сущность задачи для сохранения в БД.
    /// Быстрое создание задачи.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="maxProjectTaskId">Максимальное значение последнего Id задачи в рамках проекта.</param>
    /// <returns>Наполненная сущность задачи.</returns>
    public static ProjectTaskEntity CreateQuickProjectTask(CreateProjectManagementTaskInput projectManagementTaskInput,
        long userId, long maxProjectTaskId)
    {
        var result = new ProjectTaskEntity
        {
            Name = projectManagementTaskInput.Name,
            AuthorId = userId,
            ProjectId = projectManagementTaskInput.ProjectId,
            TaskStatusId = (int)ProjectTaskStatusEnum.New,
            Created = DateTime.UtcNow,
            TaskTypeId = (int)ProjectTaskTypeEnum.Task,
            ProjectTaskId = maxProjectTaskId
        };

        // Если к задаче добавили исполнителя.
        if (projectManagementTaskInput.ExecutorId.HasValue)
        {
            result.ExecutorId = projectManagementTaskInput.ExecutorId.Value;
        }

        // Если к задаче не добавили приоритет, то по дефолту проставим нормальный.
        if (!projectManagementTaskInput.PriorityId.HasValue)
        {
            result.PriorityId = (int)TaskPriorityEnum.Medium;
        }

        return result;
    }
    
    /// <summary>
    /// Метод создает сущность задачи для сохранения в БД.
    /// Обычное создание задачи.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="maxProjectTaskId">Максимальное значение последнего Id задачи в рамках проекта.</param>
    /// <returns>Наполненная сущность задачи.</returns>
    public static ProjectTaskEntity CreateProjectTask(CreateProjectManagementTaskInput projectManagementTaskInput,
        long userId, long maxProjectTaskId)
    {
        var result = new ProjectTaskEntity
        {
            Name = projectManagementTaskInput.Name,
            AuthorId = userId,
            ProjectId = projectManagementTaskInput.ProjectId,
            TaskStatusId = projectManagementTaskInput.TaskStatusId,
            Created = DateTime.UtcNow,
            TaskTypeId = projectManagementTaskInput.TaskTypeId,
            PriorityId = projectManagementTaskInput.PriorityId,
            ProjectTaskId = maxProjectTaskId
        };

        // Если к задаче добавили наблюдателей.
        if (projectManagementTaskInput.WatcherIds is not null && projectManagementTaskInput.WatcherIds.Any())
        {
            result.WatcherIds = projectManagementTaskInput.WatcherIds;
        }
        
        // Если к задаче добавили исполнителя.
        if (projectManagementTaskInput.ExecutorId.HasValue)
        {
            result.ExecutorId = projectManagementTaskInput.ExecutorId.Value;
        }
        
        // Если к задаче добавили описание.
        if (!string.IsNullOrWhiteSpace(projectManagementTaskInput.Details))
        {
            result.Details = projectManagementTaskInput.Details;
        }
        
        // Если к задаче добавили теги (метки).
        if (projectManagementTaskInput.TagIds is not null && projectManagementTaskInput.TagIds.Any())
        {
            result.TagIds = projectManagementTaskInput.TagIds;
        }
        
        // Если к задаче не добавили приоритет, то по дефолту проставим средний.
        if (!projectManagementTaskInput.PriorityId.HasValue)
        {
            result.PriorityId = (int)TaskPriorityEnum.Medium;
        }

        return result;
    }
    
    /// <summary>
    /// Метод создает сущность епика для сохранения в БД.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="maxProjectTaskId">Максимальное значение последнего Id задачи в рамках проекта.</param>
    /// <returns>Наполненная сущность епика.</returns>
    public static EpicEntity CreateProjectEpic(CreateProjectManagementTaskInput projectManagementTaskInput,
        long userId, long maxProjectTaskId)
    {
        var result = new EpicEntity
        {
            EpicName = projectManagementTaskInput.Name,
            CreatedBy = userId,
            ProjectId = projectManagementTaskInput.ProjectId,
            TaskStatusId = projectManagementTaskInput.TaskStatusId,
            CreatedAt = DateTime.UtcNow,
            PriorityId = projectManagementTaskInput.PriorityId!.Value,
            ProjectEpicId = maxProjectTaskId
        };

        // Если к задаче добавили описание.
        if (!string.IsNullOrWhiteSpace(projectManagementTaskInput.Details))
        {
            result.EpicDescription = projectManagementTaskInput.Details;
        }
        
        // Если к задаче добавили теги (метки).
        if (projectManagementTaskInput.TagIds is not null && projectManagementTaskInput.TagIds.Any())
        {
            result.TagIds = projectManagementTaskInput.TagIds;
        }

        // Если к задаче не добавили приоритет, то по дефолту проставим средний.
        if (!projectManagementTaskInput.PriorityId.HasValue)
        {
            result.PriorityId = (int)TaskPriorityEnum.Medium;
        }

        // Если заполнили дату начала эпика.
        if (projectManagementTaskInput.DateStart.HasValue)
        {
            result.DateStart = projectManagementTaskInput.DateStart.Value;
        }
        
        // Если заполнили дату окончания эпика.
        if (projectManagementTaskInput.DateEnd.HasValue)
        {
            result.DateEnd = projectManagementTaskInput.DateEnd.Value;
        }

        return result;
    }
    
    /// <summary>
    /// Метод создает сущность истории для сохранения в БД.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="maxProjectTaskId">Максимальное значение последнего Id задачи в рамках проекта.</param>
    /// <returns>Наполненная сущность епика.</returns>
    public static UserStoryEntity CreateProjectUserStory(CreateProjectManagementTaskInput projectManagementTaskInput,
        long userId, long maxProjectTaskId)
    {
        var result = new UserStoryEntity
        {
            StoryName = projectManagementTaskInput.Name,
            StoryDescription = projectManagementTaskInput.Details,
            CreatedBy = userId,
            ProjectId = projectManagementTaskInput.ProjectId,
            StoryStatusId = projectManagementTaskInput.TaskStatusId,
            CreatedAt = DateTime.UtcNow,
            TaskStatusId = maxProjectTaskId
        };

        // Если к задаче добавили теги (метки).
        if (projectManagementTaskInput.TagIds is not null && projectManagementTaskInput.TagIds.Any())
        {
            result.TagIds = projectManagementTaskInput.TagIds;
        }
        
        // Если к задаче добавили наблюдателей.
        if (projectManagementTaskInput.WatcherIds is not null && projectManagementTaskInput.WatcherIds.Any())
        {
            result.WatcherIds = projectManagementTaskInput.WatcherIds;
        }
        
        // Если к задаче добавили исполнителя.
        if (projectManagementTaskInput.ExecutorId.HasValue)
        {
            result.ExecutorId = projectManagementTaskInput.ExecutorId.Value;
        }
        
        // Если к задаче добавили эпик.
        if (projectManagementTaskInput.EpicId.HasValue)
        {
            result.EpicId = projectManagementTaskInput.EpicId.Value;
        }

        return result;
    }
}