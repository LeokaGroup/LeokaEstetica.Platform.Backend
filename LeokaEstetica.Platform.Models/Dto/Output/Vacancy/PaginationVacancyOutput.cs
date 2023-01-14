// using LeokaEstetica.Platform.Models.Dto.Output.Pagination;

using LeokaEstetica.Platform.Models.Dto.Output.Pagination;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели пагинации.
/// </summary>
public class PaginationVacancyOutput
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public List<CatalogVacancyOutput> Vacancies { get; set; }

    /// <summary>
    /// TODO: Пока неизвестно, возможно не нужна.
    /// Информация о пагинации.
    /// </summary>
    public PaginationInfoOutput PaginationInfo { get; set; }

    // /// <summary>
    // /// Нужно ли загрузить оставшиеся записи.
    // /// </summary>
    // public bool IsLoadAll { get; set; }
    
    /// <summary>
    /// Признак отображения пагинации.
    /// </summary>
    public bool IsVisiblePagination { get; set; }

    /// <summary>
    /// Кол-во всего.
    /// </summary>
    public int Total => Vacancies.Count;
}