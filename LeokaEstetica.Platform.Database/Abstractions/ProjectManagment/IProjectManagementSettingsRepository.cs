using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
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
    Task<IEnumerable<SprintMoveNotCompletedTaskSetting>?> GetProjectSprintsMoveNotCompletedTasksSettingsAsync(
        long projectId);

    /// <summary>
    /// Метод обновляет настройки длительности спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectId">Признак выбранной настройки.</param>
    /// <param name="sysName">Системное название настройки.</param>
    Task UpdateProjectSprintsDurationSettingsAsync(long projectId, bool IsSettingSelected, string sysName);

    /// <summary>
    /// Метод обновляет настройки перемещения нерешенных задач спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectId">Признак выбранной настройки.</param>
    /// <param name="sysName">Системное название настройки.</param>
    Task UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(long projectId, bool isSettingSelected,
        string sysName);

    /// <summary>
    /// Метод получает выбранную настройку длительности спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные настройки.</returns>
    Task<SprintDurationSetting?> GetProjectSprintDurationSettingsAsync(long projectId);

    /// <summary>
    /// Метод заводит Scrum настройки для проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    Task ConfigureProjectScrumSettingsAsync(long projectId);
    
    /// <summary>
    /// Метод получает список пользователей, которые состоят в проекте.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список пользователей.</returns>
    Task<IEnumerable<ProjectSettingUserOutput>> GetCompanyProjectUsersAsync(long projectId);
}