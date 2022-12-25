namespace LeokaEstetica.Platform.Moderation.Models.Dto.Input.Vacancy;

/// <summary>
/// Класс входной модели для апрува вакансии при модерации.
/// </summary>
public class ApproveVacancyInput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }
}