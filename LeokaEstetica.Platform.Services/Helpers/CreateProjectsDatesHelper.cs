using System.Globalization;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.Services.Helpers;

/// <summary>
/// Класс хелпера дат проектов.
/// </summary>
public static class CreateProjectsDatesHelper
{
    /// <summary>
    /// Метод форматирует даты проектов.
    /// </summary>
    /// <param name="projects">Список проектов из БД.</param>
    /// <param name="projectsArchive">Результирующий список проектов.</param>
    public static async Task CreateDatesResultAsync(IEnumerable<ArchivedProjectEntity> projects,
        List<ProjectArchiveOutput> projectsArchive)
    {
        var i = 0;
        
        foreach (var prj in projects)
        {
            projectsArchive[i].DateArchived = prj.DateArchived.ToString("g", CultureInfo.GetCultureInfo("ru"));
            i++;
        }

        await Task.CompletedTask;
    }
}