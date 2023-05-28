using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Chains.Vacancy;

/// <summary>
/// Класс фильтра по типу оплаты (без оплаты).
/// </summary>
public class NotPayVacanciesFilterChain : BaseVacanciesFilterChain
{
    /// <summary>
    /// Метод фильтрует вакансии по типу оплаты (без оплаты).
    /// </summary>
    /// <param name="filters">Условия фильтрации.</param>
    /// <param name="vacancies">Список вакансий до фильтрации без выгрузки в память.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public override async Task<IQueryable<CatalogVacancyOutput>> FilterVacanciesAsync(FilterVacancyInput filters,
        IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        // Если фильтр не имеет тип оплаты (без оплаты), то передаем следующему по цепочке.
        if (Enum.Parse<FilterPayTypeEnum>(filters.Pay) != FilterPayTypeEnum.NotPay)
        {
            return await CallNextSuccessor(filters, vacancies);
        }
        
        Initialize(vacancies);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);
        var filterQuery =
            new TermQuery(new Term(VacancyFinderConst.PAYMENT, FilterPayTypeEnum.NotPay.GetEnumDescription()));
        var filter = new QueryWrapperFilter(filterQuery);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), filter, 20).ScoreDocs;
        var result = CreateVacanciesSearchResultBuilder.CreateVacanciesSearchResult(searchResults, searcher);
        vacancies = (IOrderedQueryable<CatalogVacancyOutput>)result;

        return await CallNextSuccessor(filters, vacancies);
    }
}