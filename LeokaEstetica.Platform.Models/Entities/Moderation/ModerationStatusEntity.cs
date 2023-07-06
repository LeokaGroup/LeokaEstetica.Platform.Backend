using LeokaEstetica.Platform.Models.Entities.Communication;

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
        ModerationResumes = new HashSet<ModerationResumeEntity>();
        ProjectComments = new HashSet<ProjectCommentEntity>();
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

    /// <summary>
    /// Список вакансий модерации.
    /// </summary>
    public ICollection<ModerationVacancyEntity> ModerationVacancies { get; set; }

    /// <summary>
    /// Список анкет модерации.
    /// </summary>
    public ICollection<ModerationResumeEntity> ModerationResumes { get; set; }

    /// <summary>
    /// Комментарии проекта.
    /// </summary>
    public ICollection<ProjectCommentEntity> ProjectComments { get; set; }
}