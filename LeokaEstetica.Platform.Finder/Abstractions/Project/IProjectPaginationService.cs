using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Finder.Abstractions.Project;

/// <summary>
/// Абстракция сервиса пагинации проектов.
/// </summary>
public interface IProjectPaginationService
{
    /// <summary>
    /// Метод пагинации проектов.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список проектов.</returns>
    Task<PaginationProjectOutput> GetProjectsPaginationAsync(int page);
}