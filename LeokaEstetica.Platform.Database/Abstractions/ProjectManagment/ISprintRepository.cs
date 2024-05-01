using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

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
}