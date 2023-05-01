namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей статусов замечаний Moderation.RemarksStatuses.
/// </summary>
public class RemarkStatuseEntity
{
    public RemarkStatuseEntity()
    {
        ProjectRemarks = new HashSet<ProjectRemarkEntity>();
        VacancyRemarks = new HashSet<VacancyRemarkEntity>();
    }

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
    /// Список замечаний проектов.
    /// </summary>
    public ICollection<ProjectRemarkEntity> ProjectRemarks { get; set; }

    /// <summary>
    /// Список замечаний вакансий.
    /// </summary>
    public ICollection<VacancyRemarkEntity> VacancyRemarks { get; set; }
}