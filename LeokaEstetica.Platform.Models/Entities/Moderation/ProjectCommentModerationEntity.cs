using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей модерации комментариев к проектам Moderation.ProjectCommentsModeration.
/// </summary>
public class ProjectCommentModerationEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ModerationId { get; set; }

    /// <summary>
    /// Id комментария.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Дата модерации.
    /// </summary>
    public DateTime DateModeration { get; set; }

    /// <summary>
    /// Id статуса.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ModerationStatusEntity ModerationStatuses { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ProjectCommentEntity ProjectComment { get; set; }
}