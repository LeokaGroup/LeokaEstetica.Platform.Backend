using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;

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
    Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync(long? templateId);

    /// <summary>
    /// Метод проставляет Id шаблонов статусам для результата.
    /// </summary>
    /// <param name="templateStatuses">Список статусов.</param>
    Task SetProjectManagmentTemplateIdsAsync(List<ProjectManagmentTaskTemplateResult> templateStatuses);
    
    /// <summary>
    /// Метод проставляет Id шаблонов статусам для результата.
    /// </summary>
    /// <param name="templateStatuses">Список статусов.</param>
    Task SetProjectManagmentTemplateIdsAsync(List<TaskStatusOutput> templateStatuses);

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
    /// Метод получает список тегов проекта для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    Task<IEnumerable<ProjectTagEntity>> GetProjectTagsAsync();

    /// <summary>
    /// Метод получает список статусов задачи для выбора.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список статусов.</returns>
    Task<IEnumerable<TaskStatusOutput>> GetTaskStatusesAsync(long projectId, string account);

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
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateProjectTagAsync(string tagName, string tagDescription, long projectId, string account);

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
    Task CreateUserTaskStatusTemplateAsync(string associationStatusSysName, string statusName,
        string statusDescription, long projectId, string account);

    /// <summary>
    /// Метод получает доступные переходы в статусы задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Список доступных переходов.</returns>
    Task<IEnumerable<AvailableTaskStatusTransitionOutput>> GetAvailableTaskStatusTransitionsAsync(long projectId,
        long projectTaskId);

    /// <summary>
    /// Метод изменяет статус задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="changeStatusId">Id статуса задачи, на который нужно изменить.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    Task ChangeTaskStatusAsync(long projectId, long changeStatusId, long taskId);
    
    /// <summary>
    /// Метод обновления описание задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="changedTaskDetails">Новое описание задачи.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskDetailsAsync(long projectId, long taskId, string changedTaskDetails, string account);
    
    /// <summary>
    /// Метод обновления названия задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="changedTaskDetails">Новое название задачи.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskNameAsync(long projectId, long taskId, string changedTaskName, string account);

    /// <summary>
    /// Метод привязывает тег к задаче проекта.
    /// Выбор происходит из набора тегов проекта.
    /// </summary>
    /// <param name="tagId">Id тега, который нужно привязать к задаче.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task AttachTaskTagAsync(int tagId, long projectTaskId, long projectId, string account);
    
    /// <summary>
    /// Метод отвязывает тег от задачи проекта.
    /// </summary>
    /// <param name="tagId">Id тега, который нужно привязать к задаче.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task DetachTaskTagAsync(int tagId, long projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод привязывает наблюдателя задачи.
    /// </summary>
    /// <param name="watcherId">Id наблюдателя, которого нужно добавить к задаче..</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task AttachTaskWatcherAsync(long watcherId, long projectTaskId, long projectId, string account);
    
    /// <summary>
    /// Метод отвязывает наблюдателя задачи.
    /// </summary>
    /// <param name="watcherId">Id наблюдателя, которого нужно добавить к задаче..</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task DetachTaskWatcherAsync(long watcherId, long projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод обновляет исполнителя задачи.
    /// </summary>
    /// <param name="executorId">Id нового исполнителя задачи.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskExecutorAsync(long executorId, long projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод обновляет приоритет задачи.
    /// </summary>
    /// <param name="priorityId">Id приоритета.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskPriorityAsync(int priorityId, long projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод создает связь с задачей (в зависимости от типа связи, который передали).
    /// </summary>
    /// <param name="taskLinkInputstring">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateTaskLinkAsync(TaskLinkInput taskLinkInput, string account);

    /// <summary>
    /// Метод получает связи задачи (обычные связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDefaultAsync(long projectId, long projectTaskId,
        LinkTypeEnum linkType);

    /// <summary>
    /// Метод получает задачи проекта, которые доступны для создания связи с текущей задачей (разных типов связей).
    /// Под текущей задачей понимается задача, которую просматривает пользователь.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список задач, доступных к созданию связи.</returns>
    Task<IEnumerable<AvailableTaskLinkOutput>> GetAvailableTaskLinkAsync(long projectId, LinkTypeEnum linkType);
    
    /// <summary>
    /// Метод получает связи задачи (родительские связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkParentAsync(long projectId, long projectTaskId,
        LinkTypeEnum linkType);
    
    /// <summary>
    /// Метод получает связи задачи (дочерние связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkChildAsync(long projectId, long projectTaskId,
        LinkTypeEnum linkType);
}