﻿using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
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
    Task<TaskSprintListResult> GetSprintsAsync(long projectId);

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

    /// <summary>
    /// Метод начинает спринт.
    /// Перед началом спринта проводится ряд проверок.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task StartSprintAsync(long projectSprintId, long projectId, string account);

    /// <summary>
    /// Метод завершает спринт (ручное завершение).
    /// </summary>
    /// <param name="sprintInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель после завершения спринта.</returns>
    Task<ManualCompleteSprintOutput> ManualCompleteSprintAsync(ManualCompleteSprintInput sprintInput, string account);
    
    /// <summary>
    /// Метод получает список спринтов доступных для переноса незавершенных задач в один из них.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список спринтов.</returns>
    Task<IEnumerable<TaskSprintExtendedOutput>> GetAvailableNextSprintsAsync(long projectSprintId, long projectId);

    /// <summary>
    /// Метод исключает задачи из спринта.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="sprintTaskIds">Список Id задач в рамках проекта, которые нужно исключить из спринта.</param>
    /// <param name="account">Аккаунт.</param>
    Task ExcludeSprintTasksAsync(long sprintId, IEnumerable<string>? sprintTaskIds, string account);
    
    /// <summary>
    /// Метод включает задачи в спринт.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="sprintTaskIds">Список Id задач в рамках проекта, которые нужно включить в спринт.</param>
    /// <param name="account">Аккаунт.</param>
    Task IncludeSprintTasksAsync(long sprintId, IEnumerable<string>? sprintTaskIds, string account);

    /// <summary>
    /// Метод удаляет спринт.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="sprintTaskIds">Список Id задач в рамках проекта, которые нужно исключить из спринта.</param>
    /// <param name="account">Аккаунт.</param>
    Task RemoveSprintAsync(long projectSprintId, long projectId, IEnumerable<long>? sprintTaskIds, string account);
}