namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели архива вакансий.
/// </summary>
public class VacancyArchiveOutput
{
    ///// <summary>
    ///// Id вакансии.
    ///// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string VacancyName { get; set; }

    /// <summary>
    /// Описание вакансии.
    /// </summary>
    public string VacancyText { get; set; }

    ///// <summary>
    ///// Статус вакансии.
    ///// </summary>
    public string VacancyStatusName { get; set; }

    /// <summary>
    /// Дата архива.
    /// </summary>
    public DateTime DateArchived { get; set; }
}
