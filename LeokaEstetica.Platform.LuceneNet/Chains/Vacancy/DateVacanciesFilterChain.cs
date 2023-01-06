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
        // Если фильтр не по соответствию, то передаем следующему по цепочке.
        if (Enum.Parse<FilterSalaryTypeEnum>(filters.Salary) != FilterSalaryTypeEnum.Date)
        {
            return await CallNextSuccessor(filters, vacancies);
        }

        using var reader = IndexReader.Open(_index.Value, true);
        using var searcher = new IndexSearcher(reader);

        // Условие сортировки поля даты.
        // Управлять можно через признак в объекте SortField.
        // Если false, то по возрастанию (по дефолту), если true, то по убыванию.
        // Поставили true, так как надо, чтобы новые вакансии были выше.
        var sort = new Sort(SortField.FIELD_SCORE,
            new SortField(VacancyFinderConst.DATE_CREATED, SortField.STRING, true));

        // Больше 20 и не надо, так как есть пагинация.
        var searchResults = searcher.Search(new MatchAllDocsQuery(), null, 20, sort).ScoreDocs;
        var vacanciesItems = new List<CatalogVacancyOutput>(20);

        foreach (var item in searchResults)
        {
            var document = searcher.Doc(item.Doc);
            var vacancyId = long.Parse(document.GetField(VacancyFinderConst.VACANCY_ID).StringValue);
            var vacancyName = string.Empty;
            var vacancyText = string.Empty;
            var dateCreated = string.Empty;
            var employment = string.Empty;
            var payment = string.Empty;
            var workExperience = string.Empty;

            if (!string.IsNullOrEmpty(document.GetField(VacancyFinderConst.VACANCY_NAME).ToString()))
            {
                vacancyName = document.GetField(VacancyFinderConst.VACANCY_NAME).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(VacancyFinderConst.VACANCY_TEXT).ToString()))
            {
                vacancyText = document.GetField(VacancyFinderConst.VACANCY_TEXT).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(VacancyFinderConst.DATE_CREATED).ToString()))
            {
                dateCreated = document.GetField(VacancyFinderConst.DATE_CREATED).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(VacancyFinderConst.EMPLOYMENT).ToString()))
            {
                employment = document.GetField(VacancyFinderConst.EMPLOYMENT).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(VacancyFinderConst.PAYMENT).ToString()))
            {
                payment = document.GetField(VacancyFinderConst.PAYMENT).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(VacancyFinderConst.WORK_EXPERIENCE).ToString()))
            {
                workExperience = document.GetField(VacancyFinderConst.WORK_EXPERIENCE).StringValue;
            }

            vacanciesItems.Add(new CatalogVacancyOutput
            {
                VacancyId = vacancyId,
                VacancyName = vacancyName,
                VacancyText = vacancyText,
                DateCreated = DateTime.Parse(dateCreated),
                Employment = employment,
                Payment = payment,
                WorkExperience = workExperience
            });
        }

        vacancies = (IOrderedQueryable<CatalogVacancyOutput>)vacanciesItems;

        return await CallNextSuccessor(filters, vacancies);
    }
}