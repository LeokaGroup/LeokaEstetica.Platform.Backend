namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели архива вакансий.
/// </summary>
public class VacancyArchiveOutput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Дата архивации вакансии.
    /// </summary>
    public string DateArchived { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string VacancyName { get; set; }
}