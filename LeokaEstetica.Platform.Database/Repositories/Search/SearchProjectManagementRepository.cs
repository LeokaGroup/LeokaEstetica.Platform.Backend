using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Search;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Database.Repositories.Search;

/// <summary>
/// Класс реализует методы репозитория поиска в модуле управления проектами.
/// </summary>
internal sealed class SearchProjectManagementRepository : BaseRepository, ISearchProjectManagementRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public SearchProjectManagementRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<SearchTaskOutput>> SearchTaskAsync(string searchText, IEnumerable<long> projectIds,
        bool isById, bool isByName, bool isByDescription, long? projectTaskId = null)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var query = "SELECT name, " +
                    "project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = ANY(@projectIds) " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix," +
                    "project_id " +
                    "FROM project_management.project_tasks " +
                    "WHERE project_id = ANY(@projectIds) ";

        var parameters = new DynamicParameters();
        parameters.Add("@searchText", searchText);
        parameters.Add("@projectIds", projectIds.AsList());
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

        // Искать по Id задачи.
        if (isById)
        {
            parameters.Add("@projectTaskId", projectTaskId!.Value);
            query += "AND project_task_id = @projectTaskId";
        }
        
        // Искать по названию задачи.
        if (isByName)
        {
            query += isById
                ? " OR name LIKE '%' || @searchText || '%'"
                : " AND name LIKE '%' || @searchText || '%'";
        }
        
        // Искать по описанию задачи.
        if (isByDescription)
        {
            query += isById || isByName
                ? " OR details LIKE '%' || @searchText || '%'"
                : " AND details LIKE '%' || @searchText || '%'";
        }

        var result = await connection.QueryAsync<SearchTaskOutput>(query, parameters);

        return result;
    }
}