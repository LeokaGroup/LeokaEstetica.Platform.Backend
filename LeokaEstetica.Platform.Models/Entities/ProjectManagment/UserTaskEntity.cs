namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей задач пользователя в модуле УП.
/// </summary>
public class UserTaskEntity
{
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
    // Jsonb в виде строки (например, 1,2,3).
    /// </summary>
    public string WatcherIds { get; set; }

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
    public int ResolutionId { get; set; }

    /// <summary>
    /// Id статуса задачи.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Список Id тегов задачи. В виде Jsonb.
    /// </summary>
    public string TagIds { get; set; }

    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TaskTypeId { get; set; }

    /// <summary>
    /// Id исполнителя задачи.
    /// </summary>
    public long ExecutorId { get; set; }
}