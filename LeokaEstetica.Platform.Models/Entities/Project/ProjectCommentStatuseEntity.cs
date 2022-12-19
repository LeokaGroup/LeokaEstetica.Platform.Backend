using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей статусов комментариев проектов.
/// </summary>
public class ProjectCommentStatuseEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Id комментария.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ProjectCommentEntity ProjectComment { get; set; }
}