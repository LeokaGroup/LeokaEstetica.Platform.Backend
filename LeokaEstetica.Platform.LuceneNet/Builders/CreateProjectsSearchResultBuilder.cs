using LeokaEstetica.Platform.LuceneNet.Consts;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.LuceneNet.Builders;

/// <summary>
/// Класс билдера результата проектов.
/// </summary>
public static class CreateProjectsSearchResultBuilder
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    private static readonly List<CatalogProjectOutput> _projects = new(20);

    /// <summary>
    /// Метод создает результат поиска проектов.
    /// </summary>
    /// <param name="searchResults">Результаты поиска.</param>
    /// <param name="searcher">Поисковый индекс.</param>
    /// <returns>Список вакансий.</returns>
    public static IQueryable<CatalogProjectOutput> CreateProjectsSearchResult(ScoreDoc[] searchResults,
        IndexSearcher searcher)
    {
        _projects.Clear();

        foreach (var item in searchResults)
        {
            var document = searcher.Doc(item.Doc);
            var projectId = long.Parse(document.GetField(ProjectFinderConst.PROJECT_ID).StringValue);
            var projectName = string.Empty;
            var projectDetails = string.Empty;
            var dateCreated = string.Empty;
            var projectIcon = string.Empty;
            var hasVacancies = string.Empty;
            var stageSysName = string.Empty;

            if (!string.IsNullOrEmpty(document.GetField(ProjectFinderConst.PROJECT_NAME).ToString()))
            {
                projectName = document.GetField(ProjectFinderConst.PROJECT_NAME).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ProjectFinderConst.PROJECT_DETAILS).ToString()))
            {
                projectDetails = document.GetField(ProjectFinderConst.PROJECT_DETAILS).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ProjectFinderConst.DATE_CREATED).ToString()))
            {
                dateCreated = document.GetField(ProjectFinderConst.DATE_CREATED).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ProjectFinderConst.PROJECT_ICON)?.ToString()))
            {
                projectIcon = document.GetField(ProjectFinderConst.PROJECT_ICON).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ProjectFinderConst.HAS_VACANCIES).ToString()))
            {
                hasVacancies = document.GetField(ProjectFinderConst.HAS_VACANCIES).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ProjectFinderConst.PROJECT_STAGE_SYSNAME).ToString()))
            {
                stageSysName = document.GetField(ProjectFinderConst.PROJECT_STAGE_SYSNAME).StringValue;
            }

            _projects.Add(new CatalogProjectOutput
            {
                ProjectId = projectId,
                ProjectName = projectName,
                ProjectDetails = projectDetails,
                DateCreated = DateTime.Parse(dateCreated),
                ProjectIcon = projectIcon,
                HasVacancies = bool.Parse(hasVacancies),
                ProjectStageSysName = stageSysName
            });
        }

        return _projects.AsQueryable();
    }
}