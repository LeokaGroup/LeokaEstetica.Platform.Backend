namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс результата списка замечаний вакансии.
/// </summary>
public class VacancyRemarkResult
{
    /// <summary>
    /// Список замечаний вакансии.
    /// </summary>
    public IEnumerable<VacancyRemarkOutput> VacancyRemark { get; set; }

    /// <summary>
    /// Кол-во замечаний всего.
    /// </summary>
    public int Total => VacancyRemark.Count();
}