using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.LuceneNet.Abstractions.Vacancy;

/// <summary>
/// Абстракция сервиса поиска и фильтрации вакансий через Lucene.NET.
/// </summary>
public interface IVacancyFinderService
{
    /// <summary>
    /// Метод запускает цепочку фильтрации вакансий.
    /// </summary>
    /// <param name="filters">Условия фильтрации.</param>
    /// <param name="vacancies">Список вакансий до фильтрации без выгрузки в память.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    Task<IEnumerable<CatalogVacancyOutput>> StartFilterChainVacanciesAsync(FilterVacancyInput filters,
        IOrderedQueryable<CatalogVacancyOutput> vacancies);
}