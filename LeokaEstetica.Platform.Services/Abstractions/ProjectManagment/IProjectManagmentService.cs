using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

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
    /// <param name="strategy">Выбранная стратегия представления.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(long projectId,
        string strategy, int templateId, string account);

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync(long projectTaskId, string account, long projectId);

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateProjectTaskAsync(CreateProjectManagementTaskInput projectManagementTaskInput, string account);

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync();
}