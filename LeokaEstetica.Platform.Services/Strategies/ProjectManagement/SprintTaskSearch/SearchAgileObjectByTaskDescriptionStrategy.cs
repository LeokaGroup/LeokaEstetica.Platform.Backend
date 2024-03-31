using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска Agile-объекта по описанию Agile-объекта.
/// </summary>
internal sealed class SearchAgileObjectByTaskDescriptionStrategy : BaseSearchAgileObjectStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    public SearchAgileObjectByTaskDescriptionStrategy(IProjectManagmentRepository projectManagmentRepository) :
        base(projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByProjectTaskIdAsync(long projectTaskId,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByProjectTaskIdAsync.");
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByTaskNameAsync(string taskName,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByTaskNameAsync.");
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByTaskDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectByTaskDescriptionAsync(taskDescription,
            projectId, templateId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }

        return result;
    }
}