namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей архива вакансий Vacancies.ArchivedVacancies.
/// </summary>
public class ArchivedVacancyEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ArchiveId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Дата архива.
    /// </summary>
    public DateTime DateArchived { get; set; }

    /// <summary>
    /// FK на вакансию пользователя.
    /// </summary>
    public UserVacancyEntity UserVacancy { get; set; }
}
