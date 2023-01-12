using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Services.Vacancy;

/// <summary>
/// Класс реализует методы поискового сервиса вакансий.
/// </summary>
public class VacancyFinderService : BaseFilterChain, IVacancyFinderService
{
    private readonly IVacancyRepository _vacancyRepository;

    public VacancyFinderService(IVacancyRepository vacancyRepository)
    {
        _vacancyRepository = vacancyRepository;
    }

    /// <summary>
    /// Метод находит вакансии по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Строка поиска.</param>
    /// <returns>Список вакансий соответствующие поисковому запросу.</returns>
    public async Task<CatalogVacancyResultOutput> SearchVacanciesAsync(string searchText)
    {
        var result = new CatalogVacancyResultOutput();
        var vacancies = await _vacancyRepository.GetFiltersVacanciesAsync();

        // Получаем все вакансии из БД без выгрузки в память.
        VacanciesDocumentLoader.Load(vacancies, _index, _analyzer);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);
        var queryParser = new QueryParser(_version, VacancyFinderConst.SEARCH_FIELD, _analyzer);
        var query = queryParser.Parse(searchText);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(query, 20).ScoreDocs;
        var items = CreateVacanciesSearchResultBuilder.CreateVacanciesSearchResult(searchResults, searcher);
        result.CatalogVacancies = (IOrderedQueryable<CatalogVacancyOutput>)items;

        return result;
    }
}