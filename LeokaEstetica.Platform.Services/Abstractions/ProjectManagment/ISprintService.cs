using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса спринтов.
/// </summary>
public interface ISprintService
{
    /// <summary>
    /// Метод получает список спринтов для бэклога проекта.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Список спринтов бэклога проекта.</returns>
    Task<IEnumerable<TaskSprintExtendedOutput>> GetSprintsAsync(long projectId);

    /// <summary>
    /// Метод получает детали спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Детали спринта.</returns>
    Task<TaskSprintExtendedOutput> GetSprintAsync(long projectSprintId, long projectId, string account);

    /// <summary>
    /// Метод обновляет название спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="sprintName">Новое название спринта.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateSprintNameAsync(long projectSprintId, long projectId, string sprintName, string account);
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="sprintDetails">Новое описание спринта.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateSprintDetailsAsync(long projectSprintId, long projectId, string sprintDetails, string account);
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="executorId">Id исполнителя спринта.</param>
    /// <param name="account">Аккаунт.</param>
    Task InsertOrUpdateSprintExecutorAsync(long projectSprintId, long projectId, long executorId, string account);
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="watcherIds">Id наблюдателей спринта.</param>
    /// <param name="account">Аккаунт.</param>
    Task InsertOrUpdateSprintWatchersAsync(long projectSprintId, long projectId, IEnumerable<long> watcherIds,
        string account);
}