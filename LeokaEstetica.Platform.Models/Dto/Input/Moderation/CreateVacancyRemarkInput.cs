namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс списка замечаний вакансии входной модели.
/// </summary>
public class CreateVacancyRemarkInput
{
    /// <summary>
    /// Список замечаний.
    /// </summary>
    public IEnumerable<VacancyRemarkInput> VacanciesRemarks { get; set; }
}