namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс выходной модели списка вакансий на модерации.
/// </summary>
public class VacanciesModerationResult
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public IEnumerable<VacancyModerationOutput> Vacancies { get; set; }

    /// <summary>
    /// Всего вакансий на модерации.
    /// </summary>
    public int Total => Vacancies.Count();
}