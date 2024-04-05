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
    internal override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectIdAsync(long projectTaskId,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByProjectTaskIdAsync.");
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectNameAsync(string taskName,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByTaskNameAsync.");
    }

    /// <inheritdoc />
    internal override async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectByObjectDescriptionAsync(taskDescription,
            projectId, templateId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }

        return result;
    }
}