using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория эпиков.
/// </summary>
internal sealed class EpicRepository : BaseRepository, IEpicRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public EpicRepository(IConnectionProvider connectionProvider) 
        : base(connectionProvider)
    {
    }
    
    /// <inheritdoc />
    public async Task ExcludeEpicTasksAsync(long epicId, IEnumerable<long>? epicTaskIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@epicId", epicId);
        parameters.Add("@epicTaskIds", epicTaskIds.AsList());

        var query = "DELETE FROM project_management.epic_tasks " +
                    "WHERE project_task_id = ANY (@epicTaskIds) " +
                    "AND epic_id = @epicId";

        await connection.ExecuteAsync(query, parameters);
    }
}