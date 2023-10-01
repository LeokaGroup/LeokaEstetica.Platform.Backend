namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей статусов вакансий Vacancies.VacancyStatuses.
/// </summary>
public class VacancyStatusEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string VacancyStatusSysName { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string VacancyStatusName { get; set; }

    public UserVacancyEntity UserVacancy { get; set; }
}