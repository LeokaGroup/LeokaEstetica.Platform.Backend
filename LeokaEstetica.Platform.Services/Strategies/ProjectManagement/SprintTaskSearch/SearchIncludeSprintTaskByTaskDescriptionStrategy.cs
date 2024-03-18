using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска задачи по описанию задачи.
/// </summary>
internal sealed class SearchIncludeSprintTaskByTaskDescriptionStrategy : BaseSearchSprintTaskStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    public SearchIncludeSprintTaskByTaskDescriptionStrategy(IProjectManagmentRepository projectManagmentRepository) :
        base(projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByProjectTaskIdAsync(long projectTaskId,
        long projectId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByProjectTaskIdAsync.");
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskNameAsync(string taskName,
        long projectId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByTaskNameAsync.");
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskDescriptionAsync(
        string taskDescription, long projectId)
    {
        var result = (await ProjectManagmentRepository.SearchIncludeSprintTaskByTaskDescriptionAsync(taskDescription,
            projectId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchTaskOutput>();
        }

        return result;
    }
}