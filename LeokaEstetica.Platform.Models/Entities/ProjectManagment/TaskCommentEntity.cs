namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей комментариев к задаче.
/// </summary>
public class TaskCommentEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Комментарий.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public DateTime Created { get; set; }
    
    /// <summary>
    /// Дата обновления комментария.
    /// </summary>
    public DateTime Updated { get; set; }

    /// <summary>
    /// Id задачи, которой принадлежит комментарий.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Id пользователя, который создал комментарий.
    /// </summary>
    public long AuthorId { get; set; }

    /// <summary>
    /// FK на задачу.
    /// </summary>
    public ProjectTaskEntity UserTask { get; set; }
}