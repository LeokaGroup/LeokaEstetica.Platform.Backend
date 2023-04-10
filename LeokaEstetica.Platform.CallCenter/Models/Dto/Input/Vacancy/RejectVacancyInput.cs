namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Vacancy;

/// <summary>
/// Класс входной модели для отклонения вакансии при модерации.
/// </summary>
public class RejectVacancyInput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }
}