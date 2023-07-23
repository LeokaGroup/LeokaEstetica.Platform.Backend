namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс результата списка замечаний вакансии.
/// </summary>
public class VacancyRemarkResult
{
    /// <summary>
    /// Список замечаний вакансии.
    /// </summary>
    public IEnumerable<VacancyRemarkOutput> VacancyRemarks { get; set; }

    /// <summary>
    /// Кол-во замечаний всего.
    /// </summary>
    public int Total => VacancyRemarks.Count();
}