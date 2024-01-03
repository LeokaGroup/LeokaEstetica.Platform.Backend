﻿using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса управления проектами.
/// </summary>
public interface IProjectManagmentService
{
    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync();

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// Этот метод не заполняет доп.списки.
    /// </summary>
    /// <returns>Список элементов.</returns>
    Task<IEnumerable<ProjectManagmentHeaderEntity>> GetHeaderItemsAsync();

    /// <summary>
    /// Метод наполняет доп.списки элементов хидера.
    /// </summary>
    /// <param name="items">Список элементов.</param>
    Task<List<ProjectManagmentHeaderResult>> ModifyHeaderItemsAsync(IEnumerable<ProjectManagmentHeaderOutput> items);

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync(long? templateId);

    /// <summary>
    /// Метод проставляет Id шаблонов статусам для результата.
    /// </summary>
    /// <param name="templateStatuses">Список статусов.</param>
    Task SetProjectManagmentTemplateIdsAsync(List<ProjectManagmentTaskTemplateResult> templateStatuses);

    /// <summary>
    /// Метод получает конфигурацию рабочего пространства по выбранному шаблону.
    /// Под конфигурацией понимаются основные элементы рабочего пространства (набор задач, статусов, фильтров, колонок и тд)
    /// если выбранный шаблон это предполагает.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(long projectId,
        string account);

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync(long projectTaskId, string account, long projectId);

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync();

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель.</returns>
    Task<CreateProjectManagementTaskOutput> CreateProjectTaskAsync(
        CreateProjectManagementTaskInput projectManagementTaskInput, string account);

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync();

    /// <summary>
    /// Метод получает список тегов пользователя для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    Task<IEnumerable<UserTaskTagEntity>> GetTaskTagsAsync();

    /// <summary>
    /// Метод получает список статусов задачи для выбора.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список статусов.</returns>
    Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetTaskStatusesAsync(long projectId, string account);

    /// <summary>
    /// Метод получает пользователей, которые могут быть выбраны в качестве исполнителя задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список пользователей.</returns>
    Task<IEnumerable<ProfileInfoEntity>> GetSelectTaskExecutorsAsync(long projectId, string account);

    /// <summary>
    /// Метод создает метку (тег) для задач пользователя.
    /// </summary>
    /// <param name="tagName">Название метки (тега).</param>
    /// <param name="tagDescription">Описание метки (тега).</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateUserTaskTagAsync(string tagName, string tagDescription, string account);

    /// <summary>
    /// Метод получает список статусов для выбора для создания нового статуса.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список статусов.</returns>
    Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetSelectableTaskStatusesAsync(long projectId,
        int templateId);

    /// <summary>
    /// Метод создает новый статус шаблона пользователя учитывая ассоциацию статуса.
    /// </summary>
    /// <param name="associationStatusSysName">Название статуса ассоциации.</param>
    /// <param name="statusName">Название статуса.</param>
    /// <param name="statusDescription">Описание статуса.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateUserTaskStatusTemplateAsync(string associationStatusSysName, string statusName, string statusDescription,
        long projectId, string account);
}