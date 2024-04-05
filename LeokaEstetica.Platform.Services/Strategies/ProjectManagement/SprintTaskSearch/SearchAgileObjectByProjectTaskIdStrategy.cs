using Dapper;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Стратегия поиска Agile-объекта по Id Agile-объекта в рамках проекта.
/// </summary>
internal class SearchAgileObjectByProjectTaskIdStrategy : BaseSearchAgileObjectStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозитрий модуля УП.</param>
    internal SearchAgileObjectByProjectTaskIdStrategy(IProjectManagmentRepository projectManagmentRepository) :
        base(projectManagmentRepository)
    {
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByProjectTaskIdAsync(
        long projectTaskId, long projectId, int templateId, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectByProjectTaskIdAsync(projectTaskId,
            projectId, templateId, searchAgileObjectType))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }

        return result;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByTaskNameAsync(string taskName,
        long projectId, int templateId, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByProjectTaskIdStrategy не предполагается реализация SearchIncludeSprintTaskByTaskNameAsync.");
    }

    /// <inheritdoc />
    public override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByTaskDescriptionAsync(
        string taskDescription, long projectId, int templateId, SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        throw new NotImplementedException(
            "В стратегии SearchIncludeSprintTaskByProjectTaskIdStrategy не предполагается реализация SearchIncludeSprintTaskByTaskDescriptionAsync.");
    }
}