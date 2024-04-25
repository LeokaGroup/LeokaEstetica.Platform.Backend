using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;

namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей вакансий пользователей Vacancies.UserVacancies.
/// </summary>
public class UserVacancyEntity
{
    public UserVacancyEntity()
    {
        CatalogVacancies = new HashSet<CatalogVacancyEntity>();
        VacancyStatuses = new HashSet<VacancyStatusEntity>();
        ModerationVacancy = new HashSet<ModerationVacancyEntity>();
        ProjectVacancies = new HashSet<ProjectVacancyEntity>();
        // ProjectTeamVacancies = new List<ProjectTeamVacancyEntity>();
        ProjectTeamMembers = new List<ProjectTeamMemberEntity>();
        ArchivedVacancies = new HashSet<ArchivedVacancyEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string VacancyName { get; set; }

    /// <summary>
    /// Описание вакансии.
    /// </summary>
    public string VacancyText { get; set; }

    /// <summary>
    /// Опыт работы.
    /// Указывается текстом в виде: Требуемый опыт работы: 1–3 года.
    /// Если не указано, то выводится "Без опыта работы".
    /// </summary>
    public string? WorkExperience { get; set; }

    /// <summary>
    /// Занятость у вакансии.
    /// Например: Полная занятость, удаленная работа.
    /// Разделяется сепаратором через запятую.
    /// Если не указано, то выводится текст "Занятость не указана".
    /// </summary>
    public string? Employment { get; set; }

    /// <summary>
    /// Дата создания вакансии.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Оплата у вакансии.
    /// Если не указано, то выводится текст "Не указана".
    /// </summary>
    public string? Payment { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Требования.
    /// </summary>
    public string? Demands { get; set; }

    /// <summary>
    /// Условия.
    /// </summary>
    public string? Conditions { get; set; }

    /// <summary>
    /// Каталог вакансий.
    /// </summary>
    public ICollection<CatalogVacancyEntity> CatalogVacancies { get; set; }

    /// <summary>
    /// Статусы вакансий.
    /// </summary>
    public ICollection<VacancyStatusEntity> VacancyStatuses { get; set; }

    /// <summary>
    /// Вакансии на модерации.
    /// </summary>
    public ICollection<ModerationVacancyEntity> ModerationVacancy { get; set; }

    /// <summary>
    /// Вакансии проекта.
    /// </summary>
    public ICollection<ProjectVacancyEntity> ProjectVacancies { get; set; }
    
    /// <summary>
    /// Список вакансий в команде проекта.
    /// </summary>
    // public ICollection<ProjectTeamVacancyEntity> ProjectTeamVacancies { get; set; }
    
    /// <summary>
    /// Список участников команд проектов.
    /// </summary>
    public ICollection<ProjectTeamMemberEntity> ProjectTeamMembers { get; set; }

    /// <summary>
    /// Список архива вакансий.
    /// </summary>
    public ICollection<ArchivedVacancyEntity> ArchivedVacancies { get; set; }
}