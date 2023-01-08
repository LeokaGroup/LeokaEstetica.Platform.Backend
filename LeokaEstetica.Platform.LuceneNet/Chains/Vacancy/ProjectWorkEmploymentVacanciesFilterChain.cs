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
/// Класс фильтра вакансий по занятости (проектная занятость).
/// </summary>
public class ProjectWorkEmploymentVacanciesFilterChain : BaseVacanciesFilterChain
{
    /// <summary>
    /// Метод фильтрует вакансии по занятости (проектная занятость).
    /// </summary>
    /// <param name="filters">Условия фильтрации.</param>
    /// <param name="vacancies">Список вакансий до фильтрации без выгрузки в память.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    public override async Task<IQueryable<CatalogVacancyOutput>> FilterVacanciesAsync(FilterVacancyInput filters,
        IOrderedQueryable<CatalogVacancyOutput> vacancies)
    {
        // Если фильтр занятости не (проектная занятость), то передаем следующему по цепочке.
        if (Enum.Parse<FilterEmploymentTypeEnum>(filters.Employment) != FilterEmploymentTypeEnum.ProjectWork)
        {
            return await CallNextSuccessor(filters, vacancies);
        }
        
        Initialize(vacancies);

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);
        var filterQuery = new TermQuery(new Term(VacancyFinderConst.WORK_EXPERIENCE,
            FilterEmploymentTypeEnum.ProjectWork.GetEnumDescription()));
        var filter = new QueryWrapperFilter(filterQuery);

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), filter, 20).ScoreDocs;
        var result = CreateVacanciesSearchResultBuilder.CreateVacanciesSearchResult(searchResults, searcher);
        vacancies = (IOrderedQueryable<CatalogVacancyOutput>)result;

        return await CallNextSuccessor(filters, vacancies);
    }
}