using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска Agile-объекта по названию Agile-объекта.
/// </summary>
internal sealed class SearchAgileObjectByTaskNameStrategy : BaseSearchAgileObjectStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    internal SearchAgileObjectByTaskNameStrategy(IProjectManagmentRepository projectManagmentRepository) : base(
        projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByProjectTaskIdAsync(long projectTaskId,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskNameStrategy не предполагается реализация SearchIncludeSprintTaskByProjectTaskIdAsync.");
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByTaskNameAsync(string taskName,
        long projectId, int templateId)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectByTaskNameAsync(taskName,
            projectId, templateId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }

        return result;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByTaskDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskNameStrategy не предполагается реализация SearchIncludeSprintTaskByTaskDescriptionAsync.");
    }
}