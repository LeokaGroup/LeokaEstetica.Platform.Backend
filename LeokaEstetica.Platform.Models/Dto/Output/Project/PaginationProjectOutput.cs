using LeokaEstetica.Platform.Models.Dto.Output.Pagination;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели пагинации проектов.
/// </summary>
public class PaginationProjectOutput : BasePaginationInfo
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    public List<CatalogProjectOutput> Projects { get; set; }

    /// <summary>
    /// Кол-во всего.
    /// </summary>
    public int Total => Projects.Count;
}