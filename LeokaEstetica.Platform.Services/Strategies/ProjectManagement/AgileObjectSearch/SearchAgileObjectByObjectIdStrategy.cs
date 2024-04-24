using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.AgileObjectSearch;

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
    internal override async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectIdAsync(
        long projectTaskId, long projectId, int templateId)
    {
        var result = (await ProjectManagmentRepository.SearchAgileObjectByObjectIdAsync(projectTaskId,
            projectId, templateId))?.AsList();

        if (result is null || !result.Any())
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }

        return result;
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchAgileObjectOutput>>
        SearchAgileObjectByObjectNameAsync(string taskName, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchAgileObjectByObjectIdStrategy не предполагается реализация SearchAgileObjectByObjectName.");
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchAgileObjectByObjectIdStrategy не предполагается реализация SearchAgileObjectByObjectDescription.");
    }
}