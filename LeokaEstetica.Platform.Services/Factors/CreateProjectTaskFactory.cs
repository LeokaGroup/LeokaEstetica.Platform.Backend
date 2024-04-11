using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки для создания задачи проекта.
/// </summary>
public static class CreateProjectTaskFactory
{
    /// <summary>
    /// Метод создает сущность задачи для сохранения в БД.
    /// Быстрое создание задачи.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наполненная сущность задачи.</returns>
    public static ProjectTaskEntity CreateQuickProjectTask(CreateProjectManagementTaskInput projectManagementTaskInput,
        long userId)
    {
        var result = new ProjectTaskEntity
        {
            Name = projectManagementTaskInput.Name,
            AuthorId = userId,
            ProjectId = projectManagementTaskInput.ProjectId,
            TaskStatusId = (int)ProjectTaskStatusEnum.New,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            TaskTypeId = (int)ProjectTaskTypeEnum.Task,
            ExecutorId = userId
        };

        return result;
    }
    
    /// <summary>
    /// Метод создает сущность задачи для сохранения в БД.
    /// Обычное создание задачи.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наполненная сущность задачи.</returns>
    public static ProjectTaskEntity CreateProjectTask(CreateProjectManagementTaskInput projectManagementTaskInput,
        long userId)
    {
        var result = new ProjectTaskEntity
        {
            Name = projectManagementTaskInput.Name,
            AuthorId = userId,
            ProjectId = projectManagementTaskInput.ProjectId,
            TaskStatusId = projectManagementTaskInput.TaskStatusId!.Value,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            TaskTypeId = projectManagementTaskInput.TaskTypeId!.Value,
            ExecutorId = projectManagementTaskInput.ExecutorId!.Value
        };

        return result;
    }
}