using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория управления проектами.
/// </summary>
public interface IProjectManagmentRepository
{
    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync();

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// </summary>
    /// <returns>Список элементов.</returns>
    Task<IEnumerable<ProjectManagmentHeaderEntity>> GetHeaderItemsAsync();

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync(long? templateId);

    /// <summary>
    /// Метод получает список Id статусов, которым будем проставлять Id шаблона, к которому относятся эти статусы.
    /// </summary>
    /// <param name="templateStatusIds">Список Id статусов.</param>
    /// <returns>Словарь с Id шаблонов и статусов.</returns>
    Task<IEnumerable<GetTemplateStatusIdByStatusIdOutput>> GetTemplateStatusIdsByStatusIdsAsync(
        IEnumerable<long> templateStatusIds);
    
    /// <summary>
    /// Метод получает задачи проекта для рабочего пространства.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Задачи проекта.</returns>
    Task<IEnumerable<ProjectTaskEntity>> GetProjectTasksAsync(long projectId);
    
    /// <summary>
    /// Метод получает названия тегов (меток) задач по их Id.
    /// </summary>
    /// <param name="tagIds">Id тегов (меток) задач.</param>
    /// <returns>Словарь с тегами (метками) задач.</returns>
    Task<IDictionary<int, ProjectTagOutput>> GetTagNamesByTagIdsAsync(IEnumerable<int> tagIds);
    
    /// <summary>
    /// Метод получает названия типов задач по их Id.
    /// </summary>
    /// <param name="typeIds">Id типов задач.</param>
    /// <returns>Словарь с типами задач.</returns>
    Task<IDictionary<int, TaskTypeOutput>> GetTypeNamesByTypeIdsAsync(IEnumerable<int> typeIds);

    /// <summary>
    /// Метод получает названия резолюций задач по их Id.
    /// </summary>
    /// <param name="resolutionIds">Id резолюций задач.</param>
    /// <returns>Словарь с резолюциями задач.</returns>
    Task<IDictionary<int, TaskResolutionOutput>> GetResolutionNamesByResolutionIdsAsync(
        IEnumerable<int> resolutionIds);
    
    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    Task<ProjectTaskEntity> GetTaskDetailsByTaskIdAsync(long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод получает названия приоритетов задач по их Id.
    /// </summary>
    /// <param name="priorityIds">Id приоритетов задач.</param>
    /// <returns>Словарь с приоритетами задач.</returns>
    Task<IDictionary<int, TaskPriorityOutput>> GetPriorityNamesByPriorityIdsAsync(IEnumerable<int> priorityIds);

    /// <summary>
    /// Метод получает последний Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Последний Id задачи в рамках проекта.</returns>
    Task<long> GetLastProjectTaskIdAsync(long projectId);
    
    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync();
    
    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync();

    /// <summary>
    /// Метод получает список тегов проекта для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    Task<IEnumerable<ProjectTagEntity>> GetProjectTagsAsync();

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="task">Задача для создания.</param>
    Task CreateProjectTaskAsync (ProjectTaskEntity task);

    /// <summary>
    /// Метод получает максимальный Position у тегов задач пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Позиция последнего тега.</returns>
    Task<int> GetLastPositionUserTaskTagAsync(long userId);

    /// <summary>
    /// Метод создает тег проекта.
    /// </summary>
    /// <param name="tag">Сущность тега.</param>
    Task CreateProjectTaskTagAsync(ProjectTagEntity tag);

    /// <summary>
    /// Метод проверяет принадлежность задачи к проекту по ProjectTaskId.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> IfProjectHavingProjectTaskIdAsync(long projectId, long projectTaskId);

    /// <summary>
    /// Метод получает Id статуса задачи по Id проекта и Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Id статуса задачи.</returns>
    Task<long> GetProjectTaskStatusIdByProjectIdProjectTaskIdAsync(long projectId, long projectTaskId);

    /// <summary>
    /// Метод получает все доступные переходы в статусы задачи из промежуточной задачи.
    /// </summary>
    /// <param name="currentTaskStatusId">Id текущего статуса задачи.</param>
    /// <returns>Список переходов.</returns>
    Task<IEnumerable<long>> GetProjectManagementTransitionIntermediateTemplatesAsync(long currentTaskStatusId);

    /// <summary>
    /// Метод получает статусы из таблицы связей многие-многие, чтобы дальше работать с
    /// конкретными таблицами статусов (либо базовыми либо кастомными статусами).
    /// </summary>
    /// <param name="statusIds">Набор Id статусов, которые нужно получить.</param>
    /// <returns>Словарь с набором Id статусов.</returns>
    Task<IEnumerable<TaskStatusIntermediateTemplateCompositeOutput>>
        GetTaskStatusIntermediateTemplatesAsync(IEnumerable<long> statusIds);

    /// <summary>
    /// Метод получает все базовые статусы задач.
    /// </summary>
    /// <returns>Список статусов задач.</returns>
    Task<IDictionary<long, GetTaskStatusTemplateOutput>> GetTaskStatusTemplatesAsync();
    
    /// <summary>
    /// Метод получает все статусы задач пользователя.
    /// </summary>
    /// <returns>Список статусов задач.</returns>
    Task<IDictionary<long, UserStatuseTemplateOutput>> GetUserTaskStatusTemplatesAsync();

    /// <summary>
    /// Метод изменяет статус задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="changeStatusId">Id статуса задачи, на который нужно изменить.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    Task ChangeTaskStatusAsync(long projectId, long changeStatusId, long taskId);

    /// <summary>
    /// Метод получает данные статуса задачи по ее TaskStatusId.
    /// </summary>
    /// <param name="taskStatusId">Id статуса задачи.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Данные статуса.</returns>
    Task<ProjectManagmentTaskStatusTemplateEntity> GetTaskStatusByTaskStatusIdAsync(long taskStatusId, int templateId);

    /// <summary>
    /// Метод обновления описание задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="changedTaskDetails">Новое описание задачи.</param>
    Task UpdateTaskDetailsAsync(long projectId, long taskId, string changedTaskDetails);
    
    /// <summary>
    /// Метод обновления названия задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="changedTaskName">Новое название задачи.</param>
    Task UpdateTaskNameAsync(long projectId, long taskId, string changedTaskName);

    /// <summary>
    /// Метод привязывает тег к задаче проекта.
    /// Выбор происходит из набора тегов проекта.
    /// </summary>
    /// <param name="tagId">Id тега, который нужно привязать к задаче.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task AttachTaskTagAsync(int tagId, long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод отвязывает тег от задачи проекта.
    /// </summary>
    /// <param name="tagId">Id тега, который нужно привязать к задаче.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task DetachTaskTagAsync(int tagId, long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод создает связь с задачей (обычная связь).
    /// </summary>
    /// <param name="taskFromLink">Id задачи, от которой исходит связь.</param>
    /// <param name="taskToLink">Id задачи, которую связывают.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="projectId">Id проекта.</param>
    Task CreateTaskLinkDefaultAsync(long taskFromLink, long taskToLink, LinkTypeEnum linkType, long projectId);
}