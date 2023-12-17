using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Finder.Chains.Project;

/// <summary>
/// Класс фильтра проектов, у которых имеются вакансии.
/// </summary>
public class ProjectsVacanciesFilterChain : BaseProjectsFilterChain
{
    /// <summary>
    /// Метод фмильтрует проекты по наличию вакансий в проектах.
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <param name="projects">Список проектов.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    public override async Task<List<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        List<CatalogProjectOutput> projects)
    {
        // Если фильтр не по наличию вакансий в проектах, то передаем следующему по цепочке.
        if (!filters.IsAnyVacancies)
        {
            return await CallNextSuccessor(filters, projects);
        }

        projects = projects.Where(p => p.HasVacancies).ToList();

        return await CallNextSuccessor(filters, projects);
    }
}