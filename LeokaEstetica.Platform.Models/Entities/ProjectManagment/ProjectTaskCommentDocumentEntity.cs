namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей документов комментариев задачи.
/// </summary>
public class ProjectTaskCommentDocumentEntity
{
    /// <summary>
    /// PK. Id документа комментария.
    /// </summary>
    public long CommentDocumentId { get; set; }

    /// <summary>
    /// Id комментария.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Id документа.
    /// </summary>
    public long DocumentId { get; set; }

    /// <summary>
    /// Id комментария.
    /// </summary>
    public long ProjectId { get; set; }
    
    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Id пользователя создавшего комментарий.
    /// </summary>
    public long CreatedBy { get; set; }
}