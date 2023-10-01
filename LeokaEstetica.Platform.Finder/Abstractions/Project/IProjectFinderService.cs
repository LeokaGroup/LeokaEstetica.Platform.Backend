using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Finder.Abstractions.Project;

/// <summary>
/// Абстракция поискового сервиса проектов.
/// </summary>
public interface IProjectFinderService
{
    /// <summary>
    /// Метод находит проекты по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Строка поиска.</param>
    /// <returns>Список проектов соответствующие поисковому запросу.</returns>
    Task<CatalogProjectResultOutput> SearchProjectsAsync(string searchText);
}