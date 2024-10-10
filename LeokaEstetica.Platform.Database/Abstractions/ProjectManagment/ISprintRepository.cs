using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
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
    /// Метод получает детали спринта, в который входит задача.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Детали спринта.</returns>
    Task<TaskSprintExtendedOutput?> GetTaskSprintByProjectTaskIdAsync(long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод получает задачи спринта проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectSprintId">Id спринта проекта.</param>
    /// <param name="strategy">Стратегия пользователя.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Задачи проекта.</returns>
    Task<IEnumerable<ProjectTaskExtendedEntity>?> GetProjectSprintTasksAsync(long projectId, long projectSprintId,
        string strategy, int templateId);
    
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
    
    /// <summary>
    /// Метод проверяет, есть ли уже активный спринт у проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> CheckActiveSprintAsync(long projectId);

    /// <summary>
    /// Метод получает кол-во задач у спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Кол-во задач у спринта.</returns>
    Task<int> GetCountSprintTasksAsync(long projectSprintId, long projectId);
    
    /// <summary>
    /// Метод запускает спринт проекта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task RunSprintAsync(long projectSprintId, long projectId);
    
    /// <summary>
    /// Метод завершает спринт проекта (ручное завершение).
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task ManualCompleteSprintAsync(long projectSprintId, long projectId);

    /// <summary>
    /// Метод получает незавершенные задачи спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Незавершенные задачи спринта.</returns>
    Task<IEnumerable<long>?> GetNotCompletedSprintTasksAsync(long projectSprintId, long projectId);

    /// <summary>
    /// Метод переносит незавершенные задачи в выбранный спринт.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="projectTaskIds">Список Id задач.</param>
    Task MoveSprintTasksAsync(long sprintId, IEnumerable<long> projectTaskIds);

    /// <summary>
    /// Метод планирует новый спринт и перемещает в него незавершенные задачи из другого спринта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskIds">Список Id задач.</param>
    /// <param name="moveSprintName">Название нового спринта.</param>
    Task PlaningNewSprintAndMoveNotCompletedSprintTasksAsync(long projectId, IEnumerable<long> projectTaskIds,
        string? moveSprintName);
    
    /// <summary>
    /// Метод переносит нерешенные задачи спринта в след.спринт.
    /// </summary>
    /// <param name="projectTaskIds">Список Id задач.</param>
    /// <param name="nextProjectSprintId">Id следующего спринта проекта.</param>
    Task MoveNotCompletedSprintTasksToNextSprintAsync(IEnumerable<long> projectTaskIds, long nextProjectSprintId);
    
    /// <summary>
    /// Метод получает список спринтов доступных для переноса незавершенных задач в один из них.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список спринтов.</returns>
    Task<IEnumerable<TaskSprintExtendedOutput>> GetAvailableNextSprintsAsync(long projectSprintId, long projectId);
    
    /// <summary>
    /// Метод получает список активных спринтов у всех проектов, у которых закончился срок и завершает их.
    /// <returns>Список спринтов.</returns>
    Task<IEnumerable<SprintEndDateOutput>?> GetSprintEndDatesAsync();
    
    /// <summary>
    /// Метод завершает спринт (автоматическое завершение).
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task AutoCompleteSprintAsync(long projectSprintId, long projectId);
    
    /// <summary>
    /// Метод проверяет, есть ли уже активный спринт у проекта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id спринта проекта следующего за активным.</returns>
    Task<long?> GetNextSprintAsync(long projectSprintId, long projectId);

    /// <summary>
    /// Метод получает Id спринта (не в рамках проекта) и Id проекта,
    /// в который входит переданный Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Id спринта.</returns>
    Task<long?> GetSprintIdByProjectTaskIdProjectIdAsync(long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод исключает задачи из спринта.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="sprintTaskIds">Список Id задач, которые нужно исключить из спринта.</param>
    Task ExcludeSprintTasksAsync(long sprintId, IEnumerable<long> sprintTaskIds);
    
    /// <summary>
    /// Метод включает задачи в спринт.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="sprintTaskIds">Список Id задач, которые нужно включить в спринт.</param>
    Task IncludeSprintTasksAsync(long sprintId, IEnumerable<long> sprintTaskIds);

    /// <summary>
    /// Метод удаляет спринт.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    Task RemoveSprintAsync(long sprintId, long projectSprintId);
}   