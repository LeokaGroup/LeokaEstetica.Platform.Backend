using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

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
    public async Task<IEnumerable<TaskSprintExtendedOutput>?> GetSprintsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT s.sprint_id," +
                    " s.date_start, s.date_end," +
                    " s.sprint_goal," +
                    " s.sprint_status_id," +
                    " s.project_id," +
                    " s.project_sprint_id," +
                    " s.sprint_name, " +
                    " ss.status_name AS SprintStatusName " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_statuses AS ss " +
                    "ON s.sprint_status_id = ss.status_id " +
                    "WHERE s.project_id = @projectId";

        var result = await connection.QueryAsync<TaskSprintExtendedOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<TaskSprintExtendedOutput?> GetSprintAsync(long projectSprintId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);
        
        var query = "SELECT s.sprint_id," +
                    " s.date_start, s.date_end," +
                    " s.sprint_goal," +
                    " s.sprint_status_id," +
                    " s.project_id," +
                    " s.project_sprint_id," +
                    " s.sprint_name," +
                    " st.sprint_id," +
                    " st.project_task_id" +
                    " ss.status_name AS SprintStatusName " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_statuses AS ss " +
                    "LEFT JOIN project_management.sprint_tasks AS st " +
                    "ON s.sprint_id = st.sprint_id" +
                    "ON s.sprint_status_id = ss.status_id " +
                    "WHERE s.project_id = @projectId " +
                    "AND s.project_sprint_id = @projectSprintId";

        var result = await connection.QueryFirstOrDefaultAsync<TaskSprintExtendedOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectTaskExtendedEntity>?> GetProjectSprintTasksAsync(long projectId,
        long projectSprintId, string strategy)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@strategy", strategy);
        parameters.Add("@projectSprintId", projectSprintId);

        var query = "SELECT t.task_id," +
                    "t.task_status_id," +
                    "t.author_id," +
                    "t.watcher_ids," +
                    "CASE " +
                    "WHEN @strategy = 'sm' THEN LEFT(t.name, 40) " +
                    "WHEN @strategy = 'kn' THEN LEFT(t.name, 100) " +
                    "END AS name," +
                    "t.details AS details," +
                    "t.created," +
                    "t.updated," +
                    "t.project_id," +
                    "t.project_task_id," +
                    "t.resolution_id," +
                    "t.tag_ids," +
                    "t.task_type_id," +
                    "t.executor_id," +
                    "t.priority_id," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                    "FROM project_management.project_tasks AS t " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON st.project_task_id = t.project_task_id " +
                    "WHERE t.project_id = @projectId " +
                    "UNION " +
                    "SELECT e.epic_id," +
                    "es.status_id," +
                    "e.created_by AS author_id," +
                    "NULL," +
                    "CASE " +
                    "WHEN @strategy = 'sm' THEN LEFT(e.epic_name, 40) " +
                    "WHEN @strategy = 'kn'THEN LEFT(e.epic_name, 100) " +
                    "END AS name," +
                    "e.epic_description AS details," +
                    "e.created_at AS created," +
                    "e.updated_at AS updated," +
                    "e.project_id," +
                    "e.project_epic_id  AS project_task_id," +
                    "e.resolution_id," +
                    "e.tag_ids," +
                    "4 AS task_type_id," +
                    "e.created_by AS executor_id," +
                    "e.priority_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                    "FROM project_management.epics AS e " +
                    "INNER JOIN project_management.epic_statuses AS es " +
                    "ON e.status_id = es.status_id " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON e.project_epic_id = st.project_task_id " +
                    "WHERE e.project_id = @projectId " +
                    "UNION " +
                    "SELECT us.story_id," +
                    "us.status_id," +
                    "us.created_by AS author_id," +
                    "us.watcher_ids," +
                    "CASE " +
                    " WHEN @strategy = 'sm' THEN LEFT(us.story_name, 40) " +
                    "WHEN @strategy = 'kn' THEN LEFT(us.story_name, 100) " +
                    "END AS name," +
                    "us.story_description AS details," +
                    "us.created_at AS created," +
                    "us.updated_at AS updated," +
                    "us.project_id," +
                    "us.user_story_task_id AS project_task_id," +
                    "us.resolution_id," +
                    "us.tag_ids," +
                    "3 AS task_type_id," +
                    "us.created_by AS executor_id," +
                    "NULL," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON us.user_story_task_id = st.project_task_id " +
                    "WHERE us.project_id = @projectId ;";

        var result = await connection.QueryAsync<ProjectTaskExtendedEntity>(query, parameters);

        return result;
    }
}