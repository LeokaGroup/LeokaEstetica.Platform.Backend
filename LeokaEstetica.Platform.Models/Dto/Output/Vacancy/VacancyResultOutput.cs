using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели списка вакансий.
/// </summary>
public class VacancyResultOutput : IFrontError
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public IEnumerable<VacancyOutput> Vacancies { get; set; }

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