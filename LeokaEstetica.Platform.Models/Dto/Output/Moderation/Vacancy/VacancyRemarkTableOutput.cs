namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс выходной модели для замечаний таблицы вакансий.
/// </summary>
public class VacancyRemarkTableOutput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string VacancyName { get; set; }
}