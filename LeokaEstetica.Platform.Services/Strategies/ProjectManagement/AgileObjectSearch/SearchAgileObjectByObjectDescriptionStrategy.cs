using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.AgileObjectSearch;

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
    internal override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectIdAsync(
        long projectTaskId, long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchAgileObjectByObjectDescriptionStrategy не предполагается реализация SearchAgileObjectByObjectId.");
    }

    /// <inheritdoc />
    internal override Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectNameAsync(string taskName,
        long projectId, int templateId)
    {
        throw new NotImplementedException(
            "В стратегии SearchAgileObjectByObjectDescriptionStrategy не предполагается реализация SearchAgileObjectByObjectName.");
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