namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей каталога вакансий Vacancies.CatalogVacancies.
/// </summary>
public class CatalogVacancyEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long CatalogVacancyId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserVacancyEntity VacancyId { get; set; }
}