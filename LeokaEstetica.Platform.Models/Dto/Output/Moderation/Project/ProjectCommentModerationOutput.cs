using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс моделрации комментариев проекта.
/// </summary>
public class ProjectCommentModerationOutput
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
    public string DateModeration { get; set; }

    /// <summary>
    /// Id статуса.
    /// </summary>
    public int ModerationStatusId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public ProjectCommentOutput ProjectComment { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public string Created { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}