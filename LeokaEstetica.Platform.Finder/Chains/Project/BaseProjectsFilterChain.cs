using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Finder.Chains.Project;

/// <summary>
/// Базовый класс фильтрации проектов.
/// </summary>
public abstract class BaseProjectsFilterChain : BaseIndexRamDirectory
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
    public abstract Task<List<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters,
        List<CatalogProjectOutput> projects);

    /// <summary>
    /// Метод передает дальше по цепочке либо возвращает результат, если в цепочке больше нет обработчиков.
    /// </summary>
    protected async Task<List<CatalogProjectOutput>> CallNextSuccessor(FilterProjectInput filters,
        List<CatalogProjectOutput> projects)
    {
        return Successor != null
            ? await Successor.FilterProjectsAsync(filters, projects)
            : projects;
    }
    
    /// <summary>
    /// Метод инициализирует данными индекс в памяти.
    /// </summary>
    /// <param name="vacancies">Список вакансий.</param>
    protected void Initialize(List<CatalogProjectOutput> projects)
    {
        ProjectsDocumentLoader.Load(projects, _index, _analyzer);
    }
}