using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория спринтов.
/// </summary>
public interface ISprintRepository
{
    /// <summary>
    /// Метод получает список спринтов для бэклога проекта.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Список спринтов бэклога проекта.</returns>
    Task<IEnumerable<TaskSprintExtendedOutput>?> GetSprintsAsync(long projectId);
    
    /// <summary>
    /// Метод получает детали спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Детали спринта.</returns>
    Task<TaskSprintExtendedOutput?> GetSprintAsync(long projectSprintId, long projectId);
    
    /// <summary>
    /// Метод получает задачи спринта проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectSprintId">Id спринта проекта.</param>
    /// <param name="strategy">Стратегия пользователя.</param>
    /// <returns>Задачи проекта.</returns>
    Task<IEnumerable<ProjectTaskExtendedEntity>?> GetProjectSprintTasksAsync(long projectId, long projectSprintId,
        string strategy);
    
    /// <summary>
    /// Метод обновляет название спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="sprintName">Новое название спринта.</param>
    Task UpdateSprintNameAsync(long projectSprintId, long projectId, string sprintName);
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="sprintDetails">Новое описание спринта.</param>
    Task UpdateSprintDetailsAsync(long projectSprintId, long projectId, string sprintDetails);
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="executorId">Id исполнителя спринта.</param>
    Task InsertOrUpdateSprintExecutorAsync(long projectSprintId, long projectId, long executorId);
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="watcherIds">Id наблюдателей спринта.</param>
    Task InsertOrUpdateSprintWatchersAsync(long projectSprintId, long projectId, IEnumerable<long> watcherIds);
}