using FluentValidation.Results;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели результата архива вакансий.
/// </summary>
public class VacancyArchiveResultOutput
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public IEnumerable<VacancyArchiveOutput> Vacancies { get; set; }

    /// <summary>
    /// Кол-во вакансий.
    /// </summary>
    public int Total => Vacancies.Count();

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Признак видимости кнопок действий вакансий проектов.
    /// </summary>
    public bool IsVisibleActionVacancyButton { get; set; }
}
