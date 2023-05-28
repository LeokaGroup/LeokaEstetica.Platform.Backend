namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели для отклонения вакансии при модерации.
/// </summary>
public class RejectVacancyOutput
{
    /// <summary>
    /// Признак успешного отклонения вакансии.
    /// </summary>
    public bool IsSuccess { get; set; }
}