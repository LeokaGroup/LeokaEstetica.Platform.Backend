using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.Document;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// TODO: Разделить в рамках рефача методы на отдельные репозитории, чтобы этот репо не был таким толстым.
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
    /// Метод получает элементы панели.
    /// Панель это и хидер модуля УП и левое выдвижное меню и меню документации проектов.
    /// </summary>
    /// <returns>Список элементов.</returns>
    Task<IEnumerable<PanelEntity>> GetPanelItemsAsync();

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
    Task<IEnumerable<ProjectTaskExtendedEntity>> GetProjectTasksAsync(long projectId);

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
    Task CreateProjectTaskAsync(ProjectTaskEntity task);

    /// <summary>
    /// Метод создает задачу эпика.
    /// </summary>
    /// <param name="epic">Эпик для создания.</param>
    Task CreateProjectEpicAsync(EpicEntity epic);

    /// <summary>
    /// Метод создает историю.
    /// </summary>
    /// <param name="story">История для создания.</param>
    Task CreateProjectUserStoryAsync(UserStoryEntity story);

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
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task DetachTaskTagAsync(int tagId, long projectTaskId, long projectId);

    /// <summary>
    /// Метод обновляет приоритет задачи.
    /// </summary>
    /// <param name="priorityId">Id приоритета.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task UpdateTaskPriorityAsync(int priorityId, long projectTaskId, long projectId);

    /// <summary>
    /// Метод обновляет исполнителя задачи.
    /// </summary>
    /// <param name="executorId">Id нового исполнителя задачи.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task UpdateTaskExecutorAsync(long executorId, long projectTaskId, long projectId);

    /// <summary>
    /// Метод привязывает наблюдателя задачи.
    /// </summary>
    /// <param name="watcherId">Id наблюдателя, которого нужно добавить к задаче..</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task AttachTaskWatcherAsync(long watcherId, long projectTaskId, long projectId);

    /// <summary>
    /// Метод отвязывает наблюдателя задачи.
    /// </summary>
    /// <param name="watcherId">Id наблюдателя, которого нужно добавить к задаче..</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task DetachTaskWatcherAsync(long watcherId, long projectTaskId, long projectId);

    /// <summary>
    /// Метод создает связь с задачей (в зависимости от типа связи, который передали).
    /// </summary>
    /// <param name="taskFromLink">Id задачи, от которой исходит связь.</param>
    /// <param name="taskToLink">Id задачи, которую связывают.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="childId">Id дочерней связи.</param>
    /// <param name="parentId">Id родительской связи.</param>
    /// <param name="dependId">Id зависящей связи.</param>
    Task CreateTaskLinkAsync(long taskFromLink, long taskToLink, LinkTypeEnum linkType, long projectId, long? childId,
        long? parentId, long? dependId);

    /// <summary>
    /// Метод получает список задач по Id проекта и списку Id задач.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    /// <returns>Данные задачи.</returns>
    Task<IEnumerable<ProjectTaskExtendedEntity>> GetProjectTaskByProjectIdTaskIdsAsync(long projectId,
        IEnumerable<long> taskId);

    /// <summary>
    /// Метод получает связи задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="fromTaskId">Id задачи, которую связывают.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей.</returns>
    Task<IEnumerable<TaskLinkExtendedEntity>> GetTaskLinksByProjectIdProjectTaskIdAsync(long projectId,
        long fromTaskId, LinkTypeEnum linkType);

    /// <summary>
    /// Метод получает задачи проекта, которые доступны для создания связи с текущей задачей (разных типов связей).
    /// Под текущей задачей понимается задача, которую просматривает пользователь.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список задач, доступных к созданию связи.</returns>
    Task<IEnumerable<AvailableTaskLinkOutput>> GetAvailableTaskLinkAsync(long projectId, LinkTypeEnum linkType);

    /// <summary>
    /// Метод разрывает связь определенного типа между задачами.
    /// </summary>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="removedLinkId">Id задачи, с которой разрывается связь (задача в рамках проекта).</param>
    /// <param name="currentTaskId">Id текущей задачи (задача в рамках проекта).</param>
    /// <param name="projectId">Id проекта.</param>
    Task RemoveTaskLinkAsync(LinkTypeEnum linkType, long removedLinkId, long currentTaskId, long projectId);

    /// <summary>
    /// Метод создает документы задачи проекта.
    /// Документы уже загружены на сервер, теперь идет их создание в БД.
    /// </summary>
    /// <param name="documents">Список документов к созданию.</param>
    /// <param name="documentType">Тип документа.</param>
    Task CreateProjectTaskDocumentsAsync(IEnumerable<ProjectDocumentEntity> documents, DocumentTypeEnum documentType);

    /// <summary>
    /// Метод получает файлы задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    /// <returns>Файлы задачи.</returns>
    Task<IEnumerable<ProjectDocumentEntity>> GetProjectTaskFilesAsync(long projectId, long taskId);

    /// <summary>
    /// Метод получает название документа по его Id.
    /// </summary>
    /// <param name="documentId">Id документа.</param>
    /// <returns>Название документа.</returns>
    Task<string> GetDocumentNameByDocumentIdAsync(long documentId);

    /// <summary>
    /// Метод получает список названий документов по их Id.
    /// </summary>
    /// <param name="userDocs">Документы пользователей.</param>
    /// <returns>список названий документов.</returns>
    Task<IEnumerable<(long? UserId, string DocumentName)>> GetDocumentNameByDocumentIdsAsync(
        IEnumerable<(long? UserId, long? DocumentId)> userDocs);

    /// <summary>
    /// Метод удаляет документ по его Id.
    /// </summary>
    /// <param name="documentId">Id документа.</param>
    Task RemoveDocumentAsync(long documentId);

    /// <summary>
    /// Метод фиксирует выбранную пользователем стратегию представления.
    /// </summary>
    /// <param name="strategySysName">Системное название стратегии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    Task FixationProjectViewStrategyAsync(string strategySysName, long projectId, long userId);

    /// <summary>
    /// Метод создает комментарий задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Комментарий задачи.</param>
    /// <param name="userId">Id пользователя.</param>
    Task CreateTaskCommentAsync(long projectTaskId, long projectId, string comment, long userId);

    /// <summary>
    /// Метод получает список комментариев задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев задачи.</returns>
    Task<IEnumerable<ProjectTaskCommentExtendedEntity>> GetTaskCommentsAsync(long projectTaskId, long projectId);

    /// <summary>
    /// Метод обновляет комментарий задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="commentId">Id комментария.</param>
    /// <param name="comment">Новый комментарий.</param>
    /// <param name="userId">Id пользователя.</param>
    Task UpdateTaskCommentAsync(long projectTaskId, long projectId, long commentId, string comment, long userId);

    /// <summary>
    /// Метод удаляет комментарий задачи.
    /// </summary>
    /// <param name="commentId">Id комментария для удаления.</param>
    Task DeleteTaskCommentAsync(long commentId);

    /// <summary>
    /// Метод получает Id изображения аватара пользователя проекта.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id документа. Может вернуть null, тогда будет выбран файл по дефолту nophoto.</returns>
    Task<long?> GetUserAvatarDocumentIdByUserIdAsync(long userId, long projectId);

    /// <summary>
    /// Метод получает массив Id изображений аватара пользователей проекта.
    /// </summary>
    /// <param name="userIds">Массив Id пользователей.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id документов. Может вернуть null, тогда будет выбран файл по дефолту nophoto.</returns>
    Task<IEnumerable<(long? UserId, long? DocumentId)>> GetUserAvatarDocumentIdByUserIdsAsync(IEnumerable<long> userIds,
        long projectId);

    /// <summary>
    /// Метод получает список эпиков.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список эпиков.</returns>
    Task<IEnumerable<EpicEntity>> GetEpicsAsync(long projectId);

    /// <summary>
    /// Метод получает эпики, доступные к добавлению в них задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список эпиков.</returns>
    Task<IEnumerable<EpicEntity>> GetAvailableEpicsAsync(long projectId);

    /// <summary>
    /// Метод добавляет задачу в эпик.
    /// </summary>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    Task IncludeTaskEpicAsync(long epicId, long projectTaskId);

    /// <summary>
    /// Метод проверяет, чтобы задача уже не находилась в эпике.
    /// </summary>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Признак нахождения задачи в эпике.</returns>
    Task<bool> IfIncludedTaskEpicAsync(long epicId, long projectTaskId);

    /// <summary>
    /// Метод получает эпик, в который входит задача.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Данные эпика.</returns>
    Task<AvailableEpicOutput> GetTaskEpicAsync(long projectId, long projectTaskId);
    
    /// <summary>
    /// Метод получает список статусов истории для выбора.
    /// </summary>
    /// <returns>Список статусов истории.</returns>
    Task<IEnumerable<UserStoryStatusEntity>> GetUserStoryStatusesAsync();
    
    /// <summary>
    /// Метод планирует спринт.
    /// Добавляет задачи в спринт, если их указали при планировании спринта.
    /// </summary>
    /// <param name="planingSprintInput">Входная модель.</param>
    /// <returns>Id нового спринта.</returns>
    Task<long> PlaningSprintAsync(PlaningSprintInput planingSprintInput);
    
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта./</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByProjectTaskIdAsync(
        long projectTaskId, long projectId, int templateId);
    
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по названию задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskName">Название задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskNameAsync(string taskName, long projectId,
        int templateId);
    
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по совпадении в описании задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskDescription">Описание задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskDescriptionAsync(
        string taskDescription, long projectId, int templateId);

    Task IncludeProjectTaskSprintAsync(IEnumerable<long> projectTaskIds, long sprintId);
}