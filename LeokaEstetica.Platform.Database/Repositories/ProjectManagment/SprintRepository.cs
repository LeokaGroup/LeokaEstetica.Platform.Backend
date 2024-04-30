using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория спринтов.
/// </summary>
internal sealed class SprintRepository : BaseRepository, ISprintRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public SprintRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskSprintExtendedOutput>> GetSprintsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT s.sprint_id, s.date_start, s.date_end, s.sprint_goal, s.sprint_status_id, s.project_id," +
                    " s.sprint_name " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_statuses AS ss " +
                    "ON s.sprint_status_id = ss.status_id " +
                    "WHERE s.project_id = @projectId";

        var result = await connection.QueryAsync<TaskSprintExtendedOutput>(query, parameters);

        return result;
    }
}