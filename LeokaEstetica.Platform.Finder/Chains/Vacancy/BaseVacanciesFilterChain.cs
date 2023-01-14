using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Finder.Chains.Vacancy;

/// <summary>
/// Базовый класс фильтрации вакансий.
/// </summary>
public abstract class BaseVacanciesFilterChain : BaseIndexRamDirectory
{
    /// <summary>
    /// Метод занимается передачей по цепочке.
    /// Он будет null, если ему неизвестен следующий в цепочке либо его нет.
    /// Поэтому набор участников в цепочке определяется перед первым вызовом.
    /// </summary>
    public BaseVacanciesFilterChain Successor { get; set; }

    /// <summary>
    /// Метод фильтрует вакансии в зависимости от фильтров.
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <param name="vacancies">Список вакансий, которые еще не выгружены в память, так как мы будем их фильтровать.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public abstract Task<IQueryable<CatalogVacancyOutput>> FilterVacanciesAsync(FilterVacancyInput filters,
        IOrderedQueryable<CatalogVacancyOutput> vacancies);

    /// <summary>
    /// Метод передает дальше по цепочке либо возвращает результат, если в цепочке больше нет обработчиков.
    /// </summary>
    protected async Task<IQueryable<CatalogVacancyOutput>> CallNextSuccessor(FilterVacancyInput filters,
        IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        return Successor != null
            ? await Successor.FilterVacanciesAsync(filters, vacancies)
            : vacancies;
    }

    /// <summary>
    /// Метод инициализирует данными индекс в памяти.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    protected void Initialize(IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        VacanciesDocumentLoader.Load(vacancies, _index, _analyzer);
    }
}