using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска задачи по названию задачи.
/// </summary>
internal sealed class SearchIncludeSprintTaskByTaskNameStrategy : BaseSearchSprintTaskStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    internal SearchIncludeSprintTaskByTaskNameStrategy(IProjectManagmentRepository projectManagmentRepository) : base(
        projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByProjectTaskIdAsync(long projectTaskId,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskNameStrategy не предполагается реализация SearchIncludeSprintTaskByProjectTaskIdAsync.");
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskNameAsync(string taskName,
        long projectId, int templateId)
    {
        var result = (await ProjectManagmentRepository.SearchIncludeSprintTaskByTaskNameAsync(taskName,
            projectId, templateId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchTaskOutput>();
        }

        return result;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskNameStrategy не предполагается реализация SearchIncludeSprintTaskByTaskDescriptionAsync.");
    }
}