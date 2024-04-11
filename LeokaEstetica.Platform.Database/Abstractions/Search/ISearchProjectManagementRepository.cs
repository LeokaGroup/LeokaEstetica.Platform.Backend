using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Database.Abstractions.Search;

/// <summary>
/// Абстракция репозитория поиска в модуле управления проектами.
/// </summary>
public interface ISearchProjectManagementRepository
{
    /// <summary>
    /// Метод поиска задач.
    /// Поиск происходит по атрибутам, которые передали.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <param name="projectIds">Список Id проектов, по которым будет поиск.</param>
    /// <param name="isById">Признак поиска по Id задачи.</param>
    /// <param name="isByName">Признак поиска по названию задачи.</param>
    /// <param name="isByDescription">Признак поиска по описанию задачи.</param>
    /// <param name="projectTaskId">Id задачи.</param>
    /// <returns>Список найденных задач.</returns>
    Task<IEnumerable<SearchAgileObjectOutput>> SearchTaskAsync(string searchText, IEnumerable<long> projectIds, bool isById,
        bool isByName, bool isByDescription, long? projectTaskId = null);
}