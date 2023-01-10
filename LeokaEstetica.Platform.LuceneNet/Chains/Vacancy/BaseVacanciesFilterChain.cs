using System.Globalization;
using LeokaEstetica.Platform.LuceneNet.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace LeokaEstetica.Platform.LuceneNet.Chains.Vacancy;

/// <summary>
/// Базовый класс фильтрации вакансий.
/// </summary>
public abstract class BaseVacanciesFilterChain : BaseFilterChain
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
        using var writer = new IndexWriter(_index.Value, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

        // Перебирем все вакансии и добавляем поля для люсины. Также указываем настройку индексов.
        foreach (var vac in vacancies)
        {
            var doc = new Document();

            if (vac.VacancyId > 0)
            {
                doc.Add(new Field(VacancyFinderConst.VACANCY_ID, vac.VacancyId.ToString(),
                    Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(vac.VacancyName))
            {
                doc.Add(new Field(VacancyFinderConst.VACANCY_NAME, vac.VacancyName, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(vac.VacancyText))
            {
                doc.Add(new Field(VacancyFinderConst.VACANCY_TEXT, vac.VacancyText, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            doc.Add(new Field(VacancyFinderConst.DATE_CREATED,
                vac.DateCreated.ToString(CultureInfo.CurrentCulture), Field.Store.YES,
                Field.Index.NOT_ANALYZED_NO_NORMS));

            if (!string.IsNullOrEmpty(vac.WorkExperience))
            {
                doc.Add(new Field(VacancyFinderConst.WORK_EXPERIENCE, vac.WorkExperience, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(vac.Employment))
            {
                doc.Add(new Field(VacancyFinderConst.EMPLOYMENT, vac.Employment, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(vac.Payment))
            {
                doc.Add(new Field(VacancyFinderConst.PAYMENT, vac.Payment, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            doc.Add(new Field(VacancyFinderConst.SEARCH_FIELD, vac.VacancyName?.Trim(), Field.Store.YES,
                Field.Index.ANALYZED));

            doc.Add(new Field(VacancyFinderConst.OUTPUT_FIELD, vac.VacancyName?.Trim(), Field.Store.YES,
                Field.Index.NOT_ANALYZED));
            writer.AddDocument(doc);
        }
        
        writer.Optimize();
        writer.Flush(false, false, false);
    }
}