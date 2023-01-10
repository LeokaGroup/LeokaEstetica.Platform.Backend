using System.Globalization;
using LeokaEstetica.Platform.LuceneNet.Consts;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace LeokaEstetica.Platform.LuceneNet.Chains.Project;

/// <summary>
/// Базовый класс фильтрации проектов.
/// </summary>
public abstract class BaseProjectsFilterChain : BaseFilterChain
{
    /// <summary>
    /// Метод занимается передачей по цепочке.
    /// Он будет null, если ему неизвестен следующий в цепочке либо его нет.
    /// Поэтому набор участников в цепочке определяется перед первым вызовом.
    /// </summary>
    public BaseProjectsFilterChain Successor { get; set; }

    /// <summary>
    /// Метод фильтрует проекты в зависимости от фильтров.
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <param name="vacancies">Список проектов, которые еще не выгружены в память, так как мы будем их фильтровать.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    public abstract Task<IQueryable<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        IOrderedQueryable<CatalogProjectOutput> projects);

    /// <summary>
    /// Метод передает дальше по цепочке либо возвращает результат, если в цепочке больше нет обработчиков.
    /// </summary>
    protected async Task<IQueryable<CatalogProjectOutput>> CallNextSuccessor(FilterProjectInput filters,
        IOrderedQueryable<CatalogProjectOutput> projects)
    {
        return Successor != null
            ? await Successor.FilterProjectsAsync(filters, projects)
            : projects;
    }
    
    /// <summary>
    /// Метод инициализирует данными индекс в памяти.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    protected void Initialize(IOrderedQueryable<CatalogProjectOutput> projects)
    {
        using var writer = new IndexWriter(_index.Value, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

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