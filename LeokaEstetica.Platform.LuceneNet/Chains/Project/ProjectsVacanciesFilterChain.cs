using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using System.Linq;

namespace LeokaEstetica.Platform.LuceneNet.Chains.Project;

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
    public override async Task<IQueryable<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        IOrderedQueryable<CatalogProjectOutput> projects)
    {
        // Если фильтр не по наличию вакансий в проектах, то передаем следующему по цепочке.
        if (!filters.IsAnyVacancies)
        {
            return await CallNextSuccessor(filters, projects);
        }

        projects = (IOrderedQueryable<CatalogProjectOutput>)projects.Where(p => p.HasVacancies);

        return await CallNextSuccessor(filters, projects);
    }
}