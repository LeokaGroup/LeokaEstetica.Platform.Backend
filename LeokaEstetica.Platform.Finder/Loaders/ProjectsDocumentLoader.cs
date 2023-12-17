using System.Globalization;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace LeokaEstetica.Platform.Finder.Loaders;

/// <summary>
/// Класс загрузчика данных в документы проектов.
/// </summary>
public static class ProjectsDocumentLoader
{
    /// <summary>
    /// Метод заполняет документы проектов наполняя их данными.
    /// </summary>
    /// <param name="vacancies">Список проектов.</param>
    /// <param name="index">Индекс.</param>
    /// <param name="analyzer">Анализатор текста.</param>
    public static void Load(List<CatalogProjectOutput> projects, Lazy<RAMDirectory> index,
        StandardAnalyzer analyzer)
    {
        using var writer = new IndexWriter(index.Value, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

        // Перебирем все вакансии и добавляем поля для люсины. Также указываем настройку индексов.
        foreach (var prj in projects)
        {
            var doc = new Document();

            if (prj.ProjectId > 0)
            {
                doc.Add(new Field(ProjectFinderConst.PROJECT_ID, prj.ProjectId.ToString(),
                    Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(prj.ProjectName))
            {
                doc.Add(new Field(ProjectFinderConst.PROJECT_NAME, prj.ProjectName, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(prj.ProjectDetails))
            {
                doc.Add(new Field(ProjectFinderConst.PROJECT_DETAILS, prj.ProjectDetails, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            doc.Add(new Field(ProjectFinderConst.DATE_CREATED,
                prj.DateCreated.ToString(CultureInfo.CurrentCulture), Field.Store.YES,
                Field.Index.NOT_ANALYZED_NO_NORMS));

            if (!string.IsNullOrEmpty(prj.ProjectIcon))
            {
                doc.Add(new Field(ProjectFinderConst.PROJECT_ICON, prj.ProjectIcon, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            if (!string.IsNullOrEmpty(prj.ProjectStageSysName))
            {
                doc.Add(new Field(ProjectFinderConst.PROJECT_STAGE_SYSNAME, prj.ProjectStageSysName, Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }
            
            if (prj.UserId > 0)
            {
                doc.Add(new Field(ProjectFinderConst.USER_ID, prj.UserId.ToString(), Field.Store.YES,
                    Field.Index.NOT_ANALYZED_NO_NORMS));
            }

            doc.Add(new Field(ProjectFinderConst.HAS_VACANCIES, prj.HasVacancies.ToString(), Field.Store.YES,
                Field.Index.NOT_ANALYZED_NO_NORMS));

            doc.Add(new Field(ProjectFinderConst.SEARCH_FIELD, prj.ProjectName?.Trim(), Field.Store.YES,
                Field.Index.ANALYZED));

            doc.Add(new Field(ProjectFinderConst.OUTPUT_FIELD, prj.ProjectName?.Trim(), Field.Store.YES,
                Field.Index.NOT_ANALYZED));
            
            writer.AddDocument(doc);
        }

        writer.Optimize();
        writer.Flush(false, false, false);
    }
}