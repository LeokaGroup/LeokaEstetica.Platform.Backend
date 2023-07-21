namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс списка выходной модели вакансий, ожидающих проверки замечаний.
/// </summary>
public class AwaitingCorrectionVacancyResult
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public IEnumerable<VacancyRemarkOutput> AwaitingCorrectionVacancies { get; set; }

    /// <summary>
    /// Кол-во.
    /// </summary>
    public int Total => AwaitingCorrectionVacancies.Count();
}