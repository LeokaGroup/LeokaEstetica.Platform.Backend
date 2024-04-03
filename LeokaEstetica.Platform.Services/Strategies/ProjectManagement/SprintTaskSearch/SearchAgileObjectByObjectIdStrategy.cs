using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска задачи по Id задачи в рамках проекта.
/// </summary>
internal class SearchAgileObjectByObjectIdStrategy : BaseSearchAgileObjectStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    internal SearchAgileObjectByObjectIdStrategy(IProjectManagmentRepository projectManagmentRepository) :
        base(projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    internal override async Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectIdAsync(
        long projectTaskId, long projectId, int templateId)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectAsyncByObjectIdAsync(projectTaskId,
            projectId, templateId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchTaskOutput>();
        }

        return result;
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchTaskOutput>>
        SearchAgileObjectByObjectNameAsync(string taskName, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByProjectTaskIdStrategy не предполагается реализация SearchIncludeSprintTaskByTaskNameAsync.");
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByProjectTaskIdStrategy не предполагается реализация SearchIncludeSprintTaskByTaskDescriptionAsync.");
    }
}