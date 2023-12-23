namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели создания задачи.
/// </summary>
public class CreateProjectManagementTaskInput
{
    /// <summary>
    /// Признак быстрого создания задачи.
    /// </summary>
    public bool IsQuickCreate { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Название задачи.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание задачи.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// Id наблюдателей задачи.
    /// </summary>
    public long[] WatcherIds { get; set; }

    /// <summary>
    /// Id статуса задачи.
    /// </summary>
    public int TaskStatusId { get; set; }
    
    /// <summary>
    /// Id тегов (меток) задачи.
    /// </summary>
    public int[] TagIds { get; set; }

    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TaskTypeId { get; set; }

    /// <summary>
    /// Id исполнителя задачи.
    /// </summary>
    public long? ExecutorId { get; set; }

    /// <summary>
    /// Id приоритета задачи.
    /// </summary>
    public int? PriorityId { get; set; }
}