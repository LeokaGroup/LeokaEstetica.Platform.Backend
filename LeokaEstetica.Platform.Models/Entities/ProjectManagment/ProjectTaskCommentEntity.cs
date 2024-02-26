namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей комментариев задачи.
/// </summary>
public class ProjectTaskCommentEntity
{
    /// <summary>
    /// PK. Id комментария.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи, которой принадлежит комментарий.
    /// </summary>
    public long ProjectTaskId { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата изменения комментария.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Id пользователя создавшего комментарий.
    /// </summary>
    public long CreatedBy { get; set; }
    
    /// <summary>
    /// Id пользователя изменившего комментарий.
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Comment { get; set; }
}