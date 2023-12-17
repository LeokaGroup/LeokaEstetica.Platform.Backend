using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей задач пользователя в модуле УП.
/// </summary>
public class ProjectTaskEntity
{
    public ProjectTaskEntity()
    {
        TaskRelations = new HashSet<TaskRelationEntity>();
        TaskComments = new HashSet<TaskCommentEntity>();
        TaskHistories = new HashSet<TaskHistoryEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Id статуса задачи.
    /// </summary>
    public int TaskStatusId { get; set; }

    /// <summary>
    /// Id пользователя, который является автором задачи.
    /// </summary>
    public long AuthorId { get; set; }

    /// <summary>
    /// Id пользователей, которые являются наблюдателями задачи.
    /// </summary>
    public long[] WatcherIds { get; set; }

    /// <summary>
    /// Название задачи.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание задачи.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// Дата создания задачи.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Дата обновления задачи.
    /// </summary>
    public DateTime Updated { get; set; }

    /// <summary>
    /// Id проекта, к которому принадлежит задача.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи, в рамках проекта. Нужен, чтобы нумерация Id задачи шло в рамках каждого проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }

    /// <summary>
    /// Id резолюции.
    /// </summary>
    public int? ResolutionId { get; set; }

    /// <summary>
    /// Список Id тегов задачи.
    /// </summary>
    public int[] TagIds { get; set; }

    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TaskTypeId { get; set; }

    /// <summary>
    /// Id исполнителя задачи.
    /// </summary>
    public long ExecutorId { get; set; }

    /// <summary>
    /// FK на статус задачи.
    /// </summary>
    public TaskStatusEntity TaskStatus { get; set; }

    /// <summary>
    /// Таблица отношений связей задач.
    /// </summary>
    public IEnumerable<TaskRelationEntity> TaskRelations { get; set; }

    /// <summary>
    /// Список зависимостей.
    /// </summary>
    public IEnumerable<TaskDependencyEntity> TaskDependencies { get; set; }

    /// <summary>
    /// FK на резолюцию задачи.
    /// </summary>
    // public TaskResolutionEntity TaskResolution { get; set; }

    /// <summary>
    /// FK на проект.
    /// </summary>
    // public UserProjectEntity UserProject { get; set; }

    /// <summary>
    /// FK на тип задачи.
    /// </summary>
    // public TaskTypeEntity TaskType { get; set; }

    /// <summary>
    /// Список комментариев задачи.
    /// </summary>
    public IEnumerable<TaskCommentEntity> TaskComments { get; set; }

    /// <summary>
    /// История действий.
    /// </summary>
    public IEnumerable<TaskHistoryEntity> TaskHistories { get; set; }

    /// <summary>
    /// Приоритет задачи.
    /// </summary>
    public int? PriorityId { get; set; }
}