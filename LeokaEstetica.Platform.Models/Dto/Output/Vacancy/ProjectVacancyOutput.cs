namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели вакансии проекта.
/// </summary>
public class ProjectVacancyOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ProjectVacancyId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Название статуса вакансии.
    /// </summary>
    public string? VacancyStatusName { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string? VacancyName { get; set; }

    /// <summary>
    /// Описание вакансии.
    /// </summary>
    public string? VacancyText { get; set; }
}