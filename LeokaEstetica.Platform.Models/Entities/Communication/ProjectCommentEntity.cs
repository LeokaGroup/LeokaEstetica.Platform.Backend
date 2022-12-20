using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей комментариев к проектам Communications.ProjectComments.
/// </summary>
public class ProjectCommentEntity
{
    public ProjectCommentEntity()
    {
        ProjectCommentsModeration = new HashSet<ProjectCommentModerationEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Признак принадлежности комментария текущему пользователю.
    /// </summary>
    public bool IsMyComment { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Список комментариев на модерации.
    /// </summary>
    public ICollection<ProjectCommentModerationEntity> ProjectCommentsModeration { get; set; }
}