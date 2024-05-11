using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория настроек проекта.
/// </summary>
public interface IProjectManagementSettingsRepository
{
    /// <summary>
    /// Метод получает настройки длительности спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список настроек длительности спринтов проекта.</returns>
    Task<IEnumerable<SprintDurationSetting>> GetProjectSprintsDurationSettingsAsync(long projectId);
    
    /// <summary>
    /// Метод получает настройки автоматического перемещения нерешенных задач спринта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список настроек автоматического перемещения нерешенных задач спринта.</returns>
    Task<IEnumerable<SprintMoveNotCompletedTaskSetting>> GetProjectSprintsMoveNotCompletedTasksSettingsAsync(
        long projectId);
}