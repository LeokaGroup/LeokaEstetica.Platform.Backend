using LeokaEstetica.Platform.LuceneNet.Consts;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.LuceneNet.Builders;

/// <summary>
/// Класс билдера результата вакансий.
/// </summary>
public static class CreateVacanciesSearchResultBuilder
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    private static readonly List<CatalogVacancyOutput> _vacancies = new(20);

    /// <summary>
    /// Метод создает результат поиска вакансий.
    /// </summary>
    /// <param name="searchResults">Результаты поиска.</param>
    /// <param name="searcher">Поисковый индекс.</param>
    /// <returns>Список вакансий.</returns>
    public static IQueryable<CatalogVacancyOutput> CreateVacanciesSearchResult(ScoreDoc[] searchResults,
        IndexSearcher searcher)
    {
        _vacancies.Clear();
        
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

            _vacancies.Add(new CatalogVacancyOutput
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

        return _vacancies.AsQueryable();
    }
}