namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей статусов модерации Moderation.ModerationStatuses.
/// </summary>
public class ModerationStatusEntity
{
    public ModerationStatusEntity()
    {
        ProjectCommentsModeration = new HashSet<ProjectCommentModerationEntity>();
        ModerationProjects = new HashSet<ModerationProjectEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса модерации.
    /// </summary>
    public string StatusName { get; set; }
    
    /// <summary>
    /// истемное название статуса модерации.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Список статусов модерации.
    /// </summary>
    public ICollection<ProjectCommentModerationEntity> ProjectCommentsModeration { get; set; }

    /// <summary>
    /// Список проектов модерации.
    /// </summary>
    public ICollection<ModerationProjectEntity> ModerationProjects { get; set; }
}