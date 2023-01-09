using LeokaEstetica.Platform.LuceneNet.Builders;
using LeokaEstetica.Platform.LuceneNet.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.LuceneNet.Chains.Vacancy;

/// <summary>
/// Класс фильтрации вакансий по дате.
/// </summary>
public sealed class DateVacanciesFilterChain : BaseVacanciesFilterChain
{
    /// <summary>
    /// Метод фильтрует вакансии по дате.
    /// </summary>
    /// <param name="filters">Условия фильтрации.</param>
    /// <param name="vacancies">Список вакансий до фильтрации без выгрузки в память.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public override async Task<IQueryable<CatalogVacancyOutput>> FilterVacanciesAsync(
        FilterVacancyInput filters, IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        // Если фильтр не по дате, то передаем следующему по цепочке.
        if (Enum.Parse<FilterSalaryTypeEnum>(filters.Salary) != FilterSalaryTypeEnum.Date)
        {
            return await CallNextSuccessor(filters, vacancies);
        }
        
        Initialize(vacancies);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);

        // Если false, то по возрастанию (по дефолту), если true, то по убыванию.
        var sort = new Sort(SortField.FIELD_SCORE,
            new SortField(VacancyFinderConst.DATE_CREATED, SortField.STRING, true));

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), null, 20, sort).ScoreDocs;
        var result = CreateVacanciesSearchResultBuilder.CreateVacanciesSearchResult(searchResults, searcher);
        vacancies = (IOrderedQueryable<CatalogVacancyOutput>)result;

        return await CallNextSuccessor(filters, vacancies);
    }
}