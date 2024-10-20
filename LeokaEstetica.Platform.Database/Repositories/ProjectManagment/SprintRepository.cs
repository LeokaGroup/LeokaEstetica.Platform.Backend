﻿using System.Data;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
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

    #region Публичные методы.

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskSprintExtendedOutput>?> GetSprintsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT s.sprint_id," +
                    " to_char(s.date_start, 'dd.MM.yyyy HH24:MI') AS DateStart," +
                    " to_char(s.date_end, 'dd.MM.yyyy HH24:MI') AS DateEnd," +
                    " s.sprint_goal," +
                    " s.sprint_status_id," +
                    " s.project_id," +
                    " s.project_sprint_id," +
                    " s.sprint_name, " +
                    " s.created_at, " +
                    " ss.status_name AS SprintStatusName " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_statuses AS ss " +
                    "ON s.sprint_status_id = ss.status_id " +
                    "WHERE s.project_id = @projectId ";

        var result = (await connection.QueryAsync<TaskSprintExtendedOutput>(query, parameters))?.AsList();

        return result ?? Enumerable.Empty<TaskSprintExtendedOutput>();
    }

    /// <inheritdoc/>
    public async Task<TaskSprintExtendedOutput?> GetSprintAsync(long projectSprintId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

        var query = "SELECT s.sprint_id," +
                    " to_char(s.date_start, 'dd.MM.yyyy HH24:MI:SS') AS DateStart," +
                    " to_char(s.date_end, 'dd.MM.yyyy HH24:MI:SS') AS DateEnd," +
                    " s.sprint_goal," +
                    " s.sprint_status_id," +
                    " s.project_id," +
                    " s.project_sprint_id," +
                    " s.sprint_name," +
                    " st.sprint_id," +
                    " st.project_task_id," +
                    " ss.status_name AS SprintStatusName," +
                    " s.executor_id," +
                    " s.watcher_ids," +
                    " s.created_by," +
                    " to_char(s.created_at, 'dd.MM.yyyy HH24:MI') AS CreatedAt," +
                    " to_char(s.updated_at, 'dd.MM.yyyy HH24:MI') AS UpdatedAt," +
                    " s.updated_by," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_statuses AS ss " +
                    "ON s.sprint_status_id = ss.status_id " +
                    "LEFT JOIN project_management.sprint_tasks AS st " +
                    "ON s.sprint_id = st.sprint_id " +
                    "WHERE s.project_id = @projectId " +
                    "AND s.project_sprint_id = @projectSprintId";

        var result = await connection.QueryFirstOrDefaultAsync<TaskSprintExtendedOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<TaskSprintExtendedOutput?> GetTaskSprintByProjectTaskIdAsync(long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectTaskId", projectTaskId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

        var query = "SELECT s.sprint_id," +
                    " s.date_start, s.date_end," +
                    " s.sprint_goal," +
                    " s.sprint_status_id," +
                    " s.project_id," +
                    " s.project_sprint_id," +
                    " s.sprint_name," +
                    " st.sprint_id," +
                    " st.project_task_id," +
                    " ss.status_name AS SprintStatusName," +
                    " s.executor_id," +
                    " s.watcher_ids," +
                    " s.created_by," +
                    " to_char(s.created_at, 'dd.MM.yyyy HH24:MI') AS CreatedAt," +
                    " to_char(s.updated_at, 'dd.MM.yyyy HH24:MI') AS UpdatedAt," +
                    " s.updated_by," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_statuses AS ss " +
                    "ON s.sprint_status_id = ss.status_id " +
                    "LEFT JOIN project_management.sprint_tasks AS st " +
                    "ON s.sprint_id = st.sprint_id " +
                    "WHERE s.project_id = @projectId " +
                    "AND st.project_task_id = @projectTaskId";

        var result = await connection.QueryFirstOrDefaultAsync<TaskSprintExtendedOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectTaskExtendedEntity>?> GetProjectSprintTasksAsync(long projectId,
        long projectSprintId, string strategy, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@strategy", strategy);
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@templateId", templateId);

        var query = "SELECT t.task_id," +
                    "t.task_status_id," +
                    "t.author_id," +
                    "t.watcher_ids," +
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
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "(SELECT tpmtst.status_name " +
                    "FROM templates.project_management_task_status_templates AS tpmtst " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS tpmtsit " +
                    "ON tpmtst.status_id = tpmtsit.status_id " +
                    "WHERE t.task_status_id = tpmtst.task_status_id " +
                    "AND tpmtsit.template_id = @templateId " +
                    "AND NOT tpmtst.is_system_status) AS StatusName " +
                    "FROM project_management.project_tasks AS t " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON st.project_task_id = t.project_task_id " +
                    "WHERE t.project_id = @projectId " +
                    "AND st.sprint_id = @projectSprintId " +
                    "UNION " +
                    "SELECT e.epic_id," +
                    "es.status_id," +
                    "e.created_by AS author_id," +
                    "NULL," +
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
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "(SELECT status_name " +
                    "FROM project_management.epic_statuses " +
                    "WHERE status_id = e.status_id) AS StatusName " +
                    "FROM project_management.epics AS e " +
                    "INNER JOIN project_management.epic_statuses AS es " +
                    "ON e.status_id = es.status_id " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON e.project_epic_id = st.project_task_id " +
                    "WHERE e.project_id = @projectId " +
                    "AND st.sprint_id = @projectSprintId " +
                    "UNION " +
                    "SELECT us.story_id," +
                    "us.status_id," +
                    "us.created_by AS author_id," +
                    "us.watcher_ids," +
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
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "(SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = us.status_id) AS StatusName " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON us.user_story_task_id = st.project_task_id " +
                    "WHERE us.project_id = @projectId " +
                    "AND st.sprint_id = @projectSprintId;";

        var result = await connection.QueryAsync<ProjectTaskExtendedEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task UpdateSprintNameAsync(long projectSprintId, long projectId, string sprintName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@sprintName", sprintName);

        var query = "UPDATE project_management.sprints " +
                    "SET sprint_name = @sprintName " +
                    "WHERE project_sprint_id = @projectSprintId " +
                    "AND project_id = @projectId";
        
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task UpdateSprintDetailsAsync(long projectSprintId, long projectId, string sprintDetails)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@sprintDetails", sprintDetails);

        var query = "UPDATE project_management.sprints " +
                    "SET sprint_goal = @sprintDetails " +
                    "WHERE project_sprint_id = @projectSprintId " +
                    "AND project_id = @projectId";
        
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task InsertOrUpdateSprintExecutorAsync(long projectSprintId, long projectId, long executorId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@executorId", executorId);
        
        var query = "UPDATE project_management.sprints " +
                    "SET executor_id = @executorId " +
                    "WHERE project_sprint_id = @projectSprintId " +
                    "AND project_id = @projectId";
        
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task InsertOrUpdateSprintWatchersAsync(long projectSprintId, long projectId,
        IEnumerable<long> watcherIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@watcherIds", watcherIds.AsList());
        
        var query = "UPDATE project_management.sprints " +
                    "SET watcher_ids = @watcherIds " +
                    "WHERE project_sprint_id = @projectSprintId " +
                    "AND project_id = @projectId";
        
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckActiveSprintAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        
        var query = "SELECT EXISTS (SELECT sprint_id " +
                    "FROM project_management.sprints " +
                    "WHERE project_id = @projectId " +
                    "AND sprint_status_id  = 2)";

        var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<int> GetCountSprintTasksAsync(long projectSprintId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);

        var query = "SELECT COUNT (st.project_task_id) " +
                    "FROM project_management.sprints AS s " +
                    "LEFT JOIN project_management.sprint_tasks AS st " +
                    "ON s.project_sprint_id = st.sprint_id " +
                    "WHERE s.project_sprint_id = @projectSprintId " +
                    "AND s.project_id = @projectId";

        var result = await connection.ExecuteScalarAsync<int>(query, parameters);

        return result;
    }
    
    /// <inheritdoc/>
    public async Task RunSprintAsync(long projectSprintId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);

        var query = "UPDATE project_management.sprints " +
                    "SET sprint_status_id = 2 " +
                    "WHERE project_sprint_id = @projectSprintId " +
                    "AND project_id = @projectId";

        await connection.ExecuteAsync(query, parameters);
    }

     /// <inheritdoc/>
    public async Task ManualCompleteSprintAsync(long projectSprintId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectSprintId", projectSprintId);
        parameters.Add("@projectId", projectId);

        var query = "UPDATE project_management.sprints " +
                    "SET sprint_status_id = 3 " +
                    "WHERE project_sprint_id = @projectSprintId " +
                    "AND project_id = @projectId";

        await connection.ExecuteAsync(query, parameters);
    }

     /// <inheritdoc/>
     public async Task<IEnumerable<long>?> GetNotCompletedSprintTasksAsync(long projectSprintId, long projectId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@projectSprintId", projectSprintId);
         parameters.Add("@projectId", projectId);

         // TODO: В будущем, возможно надо будет дорабатывать под кастомные статусы системные имена.
         var query = "SELECT DISTINCT (st.project_task_id) " +
                     "FROM project_management.sprint_tasks AS st " +
                     "INNER JOIN project_management.project_tasks AS pt " +
                     "ON st.project_task_id = pt.project_task_id " +
                     "LEFT JOIN templates.project_management_task_status_templates AS pmtst " +
                     "ON pt.task_status_id = pmtst.task_status_id " +
                     "WHERE pt.project_id = @projectId " +
                     "AND st.sprint_id = @projectSprintId " +
                     "AND pmtst.task_status_id <> 6 " +
                     "AND pmtst.status_sys_name NOT IN ('Completed', 'InArchive', 'Closed')";

         var result = await connection.QueryAsync<long>(query, parameters);

         return result;
     }

     /// <inheritdoc/>
     public async Task MoveSprintTasksAsync(long sprintId, IEnumerable<long> projectTaskIds)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@sprintId", sprintId);
         parameters.Add("@projectTaskIds", projectTaskIds.AsList());

         var query = "UPDATE project_management.sprint_tasks " +
                     "SET sprint_id = @sprintId " +
                     "WHERE project_task_id = ANY (@projectTaskIds)";

         await connection.ExecuteAsync(query, parameters);
     }

     /// <inheritdoc/>
     public async Task PlaningNewSprintAndMoveNotCompletedSprintTasksAsync(long projectId,
         IEnumerable<long> projectTaskIds, string? moveSprintName)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();
         using var transaction = await ConnectionProvider.CreateTransactionAsync(IsolationLevel.ReadCommitted);

         try
         {
             // Получаем последний Id спринта в рамках проекта, перед созданим нового.
             var lastProjectSprintIdParameters = new DynamicParameters();
             lastProjectSprintIdParameters.Add("@projectId", projectId);

             var lastProjectSprintIdQuery = "SELECT MAX (project_sprint_id) " +
                                            "FROM project_management.sprints " +
                                            "WHERE project_id = @projectId";
        
             var lastProjectSprintId = await connection.ExecuteScalarAsync<long>(lastProjectSprintIdQuery,
                 lastProjectSprintIdParameters);
             var newSprintId = ++lastProjectSprintId;
             
             var parametersCreateSprint = new DynamicParameters();
             parametersCreateSprint.Add("@moveSprintName", moveSprintName);
             parametersCreateSprint.Add("@projectSprintId", newSprintId);

             var queryCreateSprint = "INSERT INTO project_management.sprints (sprint_name, project_sprint_id) " +
                                     "VALUES (@moveSprintName, @lastProjectSprintId)";
             
             // Планируем новый спринт.
             await connection.ExecuteAsync(queryCreateSprint, parametersCreateSprint);
             
             var parametersSprintTasks = new DynamicParameters();
             parametersSprintTasks.Add("@projectTaskIds", projectTaskIds.AsList());
             parametersSprintTasks.Add("@projectSprintId", newSprintId);

             var querySprintTasks = "UPDATE project_management.sprint_tasks " +
                                    "SET sprint_id = @projectSprintId " +
                                    "WHERE project_task_id = ANY (@projectTaskIds)";
             
             // Переносим новые задачи в новый спринт.
             await connection.ExecuteAsync(querySprintTasks, parametersSprintTasks);

             transaction.Commit();
         }

         catch
         {
             transaction.Rollback();
             throw;
         }
     }

     /// <inheritdoc/>
     public async Task<IEnumerable<TaskSprintExtendedOutput>> GetAvailableNextSprintsAsync(long projectSprintId,
         long projectId)
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
                     " s.sprint_name, " +
                     " ss.status_name AS SprintStatusName " +
                     "FROM project_management.sprints AS s " +
                     "INNER JOIN project_management.sprint_statuses AS ss " +
                     "ON s.sprint_status_id = ss.status_id " +
                     "WHERE s.project_id = @projectId " +
                     "AND s.project_sprint_id > @projectSprintId " +
                     "ORDER BY s.created_at DESC";

         var result = await connection.QueryAsync<TaskSprintExtendedOutput>(query, parameters);

         return result;
     }

     /// <inheritdoc/>
     public async Task<IEnumerable<SprintEndDateOutput>?> GetSprintEndDatesAsync()
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var query = "SELECT sprint_id, project_sprint_id, project_id " +
                     "FROM project_management.sprints " +
                     "WHERE date_end <= NOW() " +
                     "AND date_start IS NOT NULL " +
                     "AND date_end IS NOT NULL";

         var result = await connection.QueryAsync<SprintEndDateOutput>(query);

         return result;
     }

     /// <inheritdoc/>
     public async Task AutoCompleteSprintAsync(long projectSprintId, long projectId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@projectSprintId", projectSprintId);
         parameters.Add("@projectId", projectId);

         var query = "UPDATE project_management.sprints " +
                     "SET sprint_status_id = 3 " +
                     "WHERE project_sprint_id = @projectSprintId " +
                     "AND project_id = @projectId";

         await connection.ExecuteAsync(query, parameters);
     }

     /// <inheritdoc/>
     public async Task<long?> GetNextSprintAsync(long projectSprintId, long projectId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@nextProjectSprintId", +projectSprintId);
         parameters.Add("@projectId", projectId);

         var query = "SELECT project_sprint_id " +
                     "FROM project_management.sprints " +
                     "WHERE project_sprint_id = @nextProjectSprintId";

         var result = await connection.QueryFirstOrDefaultAsync<long?>(query, parameters);

         return result;
     }

     /// <inheritdoc/>
     public async Task<long?> GetSprintIdByProjectTaskIdProjectIdAsync(long projectTaskId, long projectId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@projectTaskId", projectTaskId);
         parameters.Add("@projectId", projectId);

         var query = "SELECT project_sprint_id " +
                     "FROM project_management.sprints AS s " +
                     "INNER JOIN project_management.sprint_tasks AS st " +
                     "ON s.sprint_id = st.sprint_id " +
                     "WHERE s.project_id = @projectId " +
                     "AND st.project_task_id = @projectTaskId";

         var result = await connection.QueryFirstOrDefaultAsync<long?>(query, parameters);

         return result;
     }

     /// <inheritdoc/>
     public async Task MoveNotCompletedSprintTasksToNextSprintAsync(IEnumerable<long> projectTaskIds,
         long nextProjectSprintId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@sprintId", nextProjectSprintId);
         parameters.Add("@projectTaskIds", projectTaskIds.AsList());

         var query = "UPDATE project_management.sprint_tasks " +
                     "SET sprint_id = @sprintId " +
                     "WHERE project_task_id = ANY (@projectTaskIds)";

         await connection.ExecuteAsync(query, parameters);
     }
     
     /// <inheritdoc/>
     public async Task ExcludeSprintTasksAsync(long sprintId, IEnumerable<long> sprintTaskIds)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@sprintId", sprintId);
         parameters.Add("@sprintTaskIds", sprintTaskIds.AsList());

         var query = "DELETE FROM project_management.sprint_tasks " +
                     "WHERE project_task_id = ANY (@sprintTaskIds) " +
                     "AND sprint_id = @sprintId";

         await connection.ExecuteAsync(query, parameters);
     }

     /// <inheritdoc/>
     public async Task IncludeSprintTasksAsync(long sprintId, IEnumerable<long> sprintTaskIds)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new List<DynamicParameters>();

         foreach (var p in sprintTaskIds)
         {
             var tempParameters = new DynamicParameters();
             tempParameters.Add("@sprintId", sprintId);
             tempParameters.Add("@projectTaskId", p);
             
             parameters.Add(tempParameters);
         }

         var query = "INSERT INTO project_management.sprint_tasks (sprint_id, project_task_id) " +
                     "VALUES (@sprintId, @projectTaskId)";

         await connection.ExecuteAsync(query, parameters);
     }

    /// <inheritdoc/>
    public async Task RemoveSprintAsync(long projectSprintId, long projectId, IEnumerable<long>? sprintTaskIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@projectSprintId", projectSprintId);
            parameters.Add("@projectId", projectId);

            // Удаляем задачи спринта если они есть.
            if (sprintTaskIds is not null && sprintTaskIds.Any())
            {
                parameters.Add("@sprintTaskIds", sprintTaskIds.AsList());

                var removeTasksSprintQuery = "DELETE FROM project_management.sprint_tasks " +
                                             "WHERE project_task_id = ANY (@sprintTaskIds) " +
                                             "AND sprint_id = @projectSprintId";

                await connection.ExecuteAsync(removeTasksSprintQuery, parameters);
            }

            // Удаляем спринт.
            var query = "DELETE FROM project_management.sprints " +
                        "WHERE project_sprint_id = @projectSprintId " +
                        "AND project_id = @projectId";

            await connection.ExecuteAsync(query, parameters);

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}