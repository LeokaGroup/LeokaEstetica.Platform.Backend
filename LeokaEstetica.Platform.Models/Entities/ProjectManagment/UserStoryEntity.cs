namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей истории пользователей.
/// </summary>
public class UserStoryEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long StoryId { get; set; }

    /// <summary>
    /// Название истории.
    /// </summary>
    public string StoryName { get; set; }

    /// <summary>
    /// Описание истории.
    /// </summary>
    public string StoryDescription { get; set; }
    
    /// <summary>
    /// Пользователь, который создал эпик.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания эпика.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления эпика.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Пользователь, который обновил эпик.
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id статуса истории.
    /// </summary>
    public int StoryStatusId { get; set; }

    /// <summary>
    /// Id наблюдателей.
    /// </summary>
    public long[] WatcherIds { get; set; }
    
    /// <summary>
    /// Список Id тегов задачи.
    /// </summary>
    public int[] TagIds { get; set; }

    /// <summary>
    /// Id эпика, в который входит история, если назначался.
    /// </summary>
    public long? EpicId { get; set; }

    /// <summary>
    /// Id исполнителя истории, если назначался.
    /// </summary>
    public long? ExecutorId { get; set; }
}