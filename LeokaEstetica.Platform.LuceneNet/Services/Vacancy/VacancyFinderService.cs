using LeokaEstetica.Platform.LuceneNet.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.LuceneNet.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса поиска и фильтрации вакансий через Lucene.NET.
/// </summary>
public sealed class VacancyFinderService : IVacancyFinderService
{
    public VacancyFinderService()
    {
    }

    /// <summary>
    /// Метод запускает цепочку фильтрации вакансий.
    /// </summary>
    /// <param name="filters">Условия фильтрации.</param>
    /// <param name="vacancies">Список вакансий до фильтрации без выгрузки в память.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public Task<IEnumerable<CatalogVacancyOutput>> StartFilterChainVacanciesAsync(FilterVacancyInput filters,
        IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        throw new NotImplementedException();
    }
}