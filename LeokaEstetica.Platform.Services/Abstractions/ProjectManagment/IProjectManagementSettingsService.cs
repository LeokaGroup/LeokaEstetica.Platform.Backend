﻿using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса настроек проекта.
/// </summary>
public interface IProjectManagementSettingsService
{
    /// <summary>
    /// Метод получает настройки длительности спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список настроек длительности спринтов проекта.</returns>
    Task<IEnumerable<SprintDurationSetting>> GetProjectSprintsDurationSettingsAsync(long projectId, string account);
    
    /// <summary>
    /// Метод получает настройки автоматического перемещения нерешенных задач спринта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список настроек автоматического перемещения нерешенных задач спринта.</returns>
    Task<IEnumerable<SprintMoveNotCompletedTaskSetting>> GetProjectSprintsMoveNotCompletedTasksSettingsAsync(
        long projectId, string account);

    /// <summary>
    /// Метод обновляет настройки длительности спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectId">Признак выбранной настройки.</param>
    /// <param name="sysName">Системное название настройки.</param>
    Task UpdateProjectSprintsDurationSettingsAsync(long projectId, bool isSettingSelected, string sysName);
    
    /// <summary>
    /// Метод обновляет настройки перемещения нерешенных задач спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectId">Признак выбранной настройки.</param>
    /// <param name="sysName">Системное название настройки.</param>
    Task UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(long projectId, bool isSettingSelected,
        string sysName);
}