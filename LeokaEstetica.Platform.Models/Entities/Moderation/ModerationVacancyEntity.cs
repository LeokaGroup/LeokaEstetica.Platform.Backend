using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей модерации вакансий Moderation.Vacancies.
/// </summary>
public class ModerationVacancyEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ModerationId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// FK на Id вакансии.
    /// </summary>
    public UserVacancyEntity UserVacancy { get; set; }

    /// <summary>
    /// Дата модерации.
    /// </summary>
    public DateTime DateModeration { get; set; }
}