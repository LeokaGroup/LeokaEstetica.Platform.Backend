using Dapper;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска задачи по описанию задачи.
/// </summary>
internal class SearchAgileObjectByObjectDescriptionStrategy : BaseSearchAgileObjectStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    public SearchAgileObjectByObjectDescriptionStrategy(IProjectManagmentRepository projectManagmentRepository) :
        base(projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectIdAsync(
        long projectTaskId, long projectId, int templateId, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByProjectTaskIdAsync.");
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectNameAsync(string taskName,
        long projectId, int templateId, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByTaskDescriptionStrategy не предполагается реализация SearchIncludeSprintTaskByTaskNameAsync.");
    }

    /// <inheritdoc />
    internal override async Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectDescriptionAsync(
        string taskDescription, long projectId, int templateId, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectByObjectDescriptionAsync(taskDescription,
            projectId, templateId, searchAgileObjectType))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchTaskOutput>();
        }

        return result;
    }
}