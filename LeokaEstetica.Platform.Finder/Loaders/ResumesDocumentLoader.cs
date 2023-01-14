using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace LeokaEstetica.Platform.Finder.Loaders;

/// <summary>
/// Класс загрузчика данных в документы резюме.
/// </summary>
public static class ResumesDocumentLoader
{
    /// <summary>
    /// Метод заполняет документы резюме наполняя их данными.
    /// </summary>
    /// <param name="resumes">Список вакансий.</param>
    /// <param name="index">Индекс.</param>
    /// <param name="analyzer">Анализатор текста.</param>
    public static void Load(IOrderedQueryable<ProfileInfoEntity> resumes, Lazy<RAMDirectory> index,
        StandardAnalyzer analyzer)
    {
        using var writer = new IndexWriter(index.Value, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

        // Перебирем все резюме и добавляем поля для люсины. Также указываем настройку индексов.
        foreach (var res in resumes)
        {
            var doc = new Document();

            if (res.UserId > 0)
            {
                doc.Add(new Field(ResumeFinderConst.USER_ID, res.UserId.ToString(),
                    Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(res.LastName))
            {
                doc.Add(new Field(ResumeFinderConst.LAST_NAME, res.LastName, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(res.FirstName))
            {
                doc.Add(new Field(ResumeFinderConst.FIRST_NAME, res.FirstName, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(res.Patronymic))
            {
                doc.Add(new Field(ResumeFinderConst.PATRONYMIC, res.Patronymic, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(res.Aboutme))
            {
                doc.Add(new Field(ResumeFinderConst.ABOUT_ME, res.Aboutme, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }
            
            doc.Add(new Field(ResumeFinderConst.IS_SHORT_FIRST_NAME, res.IsShortFirstName.ToString(), Field.Store.YES,
                Field.Index.NOT_ANALYZED_NO_NORMS));

            if (!string.IsNullOrEmpty(res.Job))
            {
                doc.Add(new Field(ResumeFinderConst.JOB, res.Job, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            doc.Add(new Field(ResumeFinderConst.SEARCH_FIELD,
                (res.LastName + " " + res.FirstName + " " + res.Patronymic).Trim(), Field.Store.YES,
                Field.Index.ANALYZED));

            doc.Add(new Field(ResumeFinderConst.OUTPUT_FIELD,
                (res.LastName + " " + res.FirstName + " " + res.Patronymic).Trim(), Field.Store.YES,
                Field.Index.NOT_ANALYZED));
            writer.AddDocument(doc);
        }

        writer.Optimize();
        writer.Flush(false, false, false);
    }
}