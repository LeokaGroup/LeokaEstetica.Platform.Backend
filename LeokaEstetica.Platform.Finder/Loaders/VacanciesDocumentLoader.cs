using System.Globalization;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace LeokaEstetica.Platform.Finder.Loaders;

/// <summary>
/// Класс загрузчика данных в документы вакансий.
/// </summary>
public static class VacanciesDocumentLoader
{
    /// <summary>
    /// Метод заполняет документы вакансий наполняя их данными.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    /// <param name="index">Индекс.</param>
    /// <param name="analyzer">Анализатор текста.</param>
    public static void Load(IOrderedQueryable<CatalogVacancyOutput> vacancies, Lazy<RAMDirectory> index,
        StandardAnalyzer analyzer)
    {
        using var writer = new IndexWriter(index.Value, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

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