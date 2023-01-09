using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.LuceneNet.Builders;
using LeokaEstetica.Platform.LuceneNet.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.LuceneNet.Chains.Vacancy;

/// <summary>
/// Класс фильтра по типу оплаты (есть оплата).
/// </summary>
public class PayVacanciesFilterChain : BaseVacanciesFilterChain
{
    /// <summary>
    /// Метод фильтрует вакансии по типу оплаты (есть оплата).
    /// </summary>
    /// <param name="filters">Условия фильтрации.</param>
    /// <param name="vacancies">Список вакансий до фильтрации без выгрузки в память.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public override async Task<IQueryable<CatalogVacancyOutput>> FilterVacanciesAsync(FilterVacancyInput filters, IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        // Если фильтр не имеет тип оплаты (есть оплата), то передаем следующему по цепочке.
        if (Enum.Parse<FilterPayTypeEnum>(filters.Pay) != FilterPayTypeEnum.Pay)
        {
            return await CallNextSuccessor(filters, vacancies);
        }
        
        Initialize(vacancies);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);
        var filterQuery =
            new TermQuery(new Term(VacancyFinderConst.PAYMENT, FilterPayTypeEnum.Pay.GetEnumDescription()));
        var filter = new QueryWrapperFilter(filterQuery);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), filter, 20).ScoreDocs;
        var result = CreateVacanciesSearchResultBuilder.CreateVacanciesSearchResult(searchResults, searcher);
        vacancies = (IOrderedQueryable<CatalogVacancyOutput>)result;

        return await CallNextSuccessor(filters, vacancies);
    }
}