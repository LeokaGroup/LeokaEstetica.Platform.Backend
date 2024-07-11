using LeokaEstetica.Platform.Models.Dto.Output.Pagination;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели пагинации вакансий.
/// </summary>
public class PaginationVacancyOutput : BasePaginationInfo
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public List<CatalogVacancyOutput>? Vacancies { get; set; }

    /// <summary>
    /// Кол-во всего.
    /// </summary>
    public int Total => Vacancies?.Count ?? 0;
}