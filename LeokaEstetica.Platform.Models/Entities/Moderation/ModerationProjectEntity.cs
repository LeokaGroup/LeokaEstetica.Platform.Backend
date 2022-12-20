using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей модерации проектов пользователей Moderation.Projects.
/// </summary>
public class ModerationProjectEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ModerationId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Дата отправки проекта на модерацию.
    /// </summary>
    public DateTime DateModeration { get; set; }

    /// <summary>
    /// Системное название статуса модерации проекта.
    /// </summary>
    public string ModerationSysName { get; set; }

    public virtual UserProjectEntity UserProject { get; set; }
}