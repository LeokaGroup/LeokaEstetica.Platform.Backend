using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

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
        ArchivedProjects = new HashSet<ArchivedProjectEntity>();
        ProjectRemarks = new HashSet<ProjectRemarkEntity>();
        VacancyRemarks = new HashSet<VacancyRemarkEntity>();
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
    /// Публичный код проекта для публичного отображения.
    /// </summary>
    public Guid PublicId { get; set; }
    
    /// <summary>
    /// Условия проекта.
    /// </summary>
    public string Conditions { get; set; }

    /// <summary>
    /// Требования проекта.
    /// </summary>
    public string Demands { get; set; }

    /// <summary>
    /// FK каталога проектов.
    /// </summary>
    public ICollection<CatalogProjectEntity> CatalogProjects { get; set; }
    
    /// <summary>
    /// Список проектов на модерации.
    /// </summary>
    public ICollection<ModerationProjectEntity> ModerationProjects { get; set; }
    
    /// <summary>
    /// Список статусов проектов.
    /// </summary>
    public ICollection<ProjectStatusEntity> ProjectStatuses { get; set; }

    /// <summary>
    /// Список стадий проектов пользователя.
    /// </summary>
    public ICollection<UserProjectStageEntity> UserProjectsStages { get; set; }

    /// <summary>
    /// Список проектов в архиве.
    /// </summary>
    public ICollection<ArchivedProjectEntity> ArchivedProjects { get; set; }

    /// <summary>
    /// Список замечаний проекта.
    /// </summary>
    public ICollection<ProjectRemarkEntity> ProjectRemarks { get; set; }

    /// <summary>
    /// Список замечаний вакансии.
    /// </summary>
    public ICollection<VacancyRemarkEntity> VacancyRemarks { get; set; }

    /// <summary>
    /// FK на задачу.
    /// </summary>
    public ProjectTaskEntity ProjectTask { get; set; }

    /// <summary>
    /// Id шаблона, если выбран был.
    /// </summary>
    public int? TemplateId { get; set; }
}