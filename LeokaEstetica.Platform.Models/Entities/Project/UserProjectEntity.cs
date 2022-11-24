using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей прооектов пользователя.
/// </summary>
public class UserProjectEntity
{
    public UserProjectEntity()
    {
        CatalogProjects = new HashSet<CatalogProjectEntity>();
        ModerationProjects = new HashSet<ModerationProjectEntity>();
        ProjectStatuses = new HashSet<ProjectStatusEntity>();
        UserProjectsStages = new HashSet<UserProjectStageEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long ProjectId { get; set; }
    
    /// <summary>
    /// Id пользователя, который создал проект (т.е владельца проекта).
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Описание проекта.
    /// </summary>
    public string ProjectDetails { get; set; }

    /// <summary>
    /// Изображение проекта.
    /// </summary>
    public string ProjectIcon { get; set; }

    /// <summary>
    /// Код проекта.
    /// </summary>
    public Guid ProjectCode { get; set; }

    /// <summary>
    /// Дата создания проекта.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// FK каталога проектов.
    /// </summary>
    public ICollection<CatalogProjectEntity> CatalogProjects { get; set; }
    
    public ICollection<ModerationProjectEntity> ModerationProjects { get; set; }
    
    public ICollection<ProjectStatusEntity> ProjectStatuses { get; set; }

    public ICollection<UserProjectStageEntity> UserProjectsStages { get; set; }
}