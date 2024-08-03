using System.Data;
using System.Text;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Document;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.Document;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;
using SqlKata;
using SqlKata.Compilers;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория управления проектами.
/// </summary>
internal sealed class ProjectManagmentRepository : BaseRepository, IProjectManagmentRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public ProjectManagmentRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    public async Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var query = @"SELECT strategy_id, view_strategy_name, view_strategy_sys_name, position 
                      FROM project_management.view_strategies 
                      ORDER BY position";

        var result = await connection.QueryAsync<ViewStrategyEntity>(query);

        return result;
    }

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// </summary>
    /// <returns>Список элементов.</returns>
    public async Task<IEnumerable<PanelEntity>> GetPanelItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var query = @"SELECT panel_id,
                             item_name,
                             item_url,
                             position,
                             panel_type,
                             items,
                             has_items,
                             is_disabled,
                             control_type,
                             destination 
                             FROM project_management.panel_items 
                             GROUP BY panel_type, panel_id 
                             ORDER BY position ";

        var result = await connection.QueryAsync<PanelEntity>(query);

        return result;
    }

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync(
        long? templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var query = @"SELECT tsit.status_id, 
                      tsit.template_id, 
                      tsit.is_custom_status, 
                      tst.status_name, 
                      tst.status_sys_name,
                      tst.task_status_id,
                      ptt.template_name 
                      FROM templates.project_management_task_status_intermediate_templates AS tsit 
                      INNER JOIN templates.project_management_task_status_templates AS tst 
                        ON tsit.status_id = tst.status_id 
                      INNER JOIN templates.project_management_task_templates AS ptt 
                        ON tsit.template_id = ptt.template_id";

        IEnumerable<ProjectManagmentTaskTemplateEntityResult> result;

        if (templateId.HasValue)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@templateId", templateId.Value);
            query += " WHERE tsit.template_id = @templateId";
            
            result = await connection.QueryAsync<ProjectManagmentTaskTemplateEntityResult>(query, parameters);
        }

        else
        {
            result = await connection.QueryAsync<ProjectManagmentTaskTemplateEntityResult>(query);
        }

        return result;
    }

    /// <summary>
    /// Метод получает список Id статусов, которым будем проставлять Id шаблона, к которому относятся эти статусы.
    /// </summary>
    /// <param name="templateStatusIds">Список Id статусов.</param>
    /// <returns>Словарь с Id шаблонов и статусов.</returns>
    public async Task<IEnumerable<GetTemplateStatusIdByStatusIdOutput>> GetTemplateStatusIdsByStatusIdsAsync(
        IEnumerable<long> templateStatusIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_intermediate_templates")
            .WhereIn("status_id", templateStatusIds)
            .Select("status_id", "template_id")
            .OrderBy("template_id");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<GetTemplateStatusIdByStatusIdOutput>(sql);

        return result;
    }

    /// <summary>
    /// Метод получает задачи проекта для рабочего пространства.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Задачи проекта.</returns>
    public async Task<IEnumerable<ProjectTaskExtendedEntity>?> GetProjectTasksAsync(long projectId, string strategy)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@strategy", strategy);

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
                    "2 AS task_type_id," +
                    "us.created_by AS executor_id," +
                    "NULL," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "WHERE us.project_id = @projectId;";

        var result = await connection.QueryAsync<ProjectTaskExtendedEntity>(query, parameters);

        return result;
    }

    /// <summary>
    /// Метод получает названия тегов (меток) пользователя задач по их Id.
    /// </summary>
    /// <param name="tagIds">Id тегов (меток) задач.</param>
    /// <returns>Словарь с тегами (метками) задач.</returns>
    public async Task<IDictionary<int, ProjectTagOutput>> GetTagNamesByTagIdsAsync(IEnumerable<int> tagIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tags")
            .WhereIn("tag_id", tagIds)
            .Select("tag_id", "tag_name", "position") // TODO: Добавить поле тип объекта из енамки.
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();
        
        // TODO: Добавить поле тип объекта из енамки.
        var result = (await connection.QueryAsync<ProjectTagOutput>(sql))
            .ToDictionary(k => k.TagId, v => new ProjectTagOutput
            {
                TagName = v.TagName,
                TagSysName = v.TagSysName,
                TagDescription = v.TagDescription,
                TagId = v.TagId,
                ProjectId = v.ProjectId
            });

        return result;
    }

    /// <summary>
    /// Метод получает названия типов задач по их Id.
    /// </summary>
    /// <param name="typeIds">Id типов задач.</param>
    /// <returns>Словарь с типами задач.</returns>
    public async Task<IDictionary<int, TaskTypeOutput>> GetTypeNamesByTypeIdsAsync(IEnumerable<int> typeIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.task_types")
            .WhereIn("type_id", typeIds)
            .Select("type_id", "type_name", "position")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();
        
        var result = (await connection.QueryAsync<TaskTypeOutput>(sql))
            .ToDictionary(k => k.TypeId, v => new TaskTypeOutput
            {
                TypeName = v.TypeName,
                TypeSysName = v.TypeSysName,
                TypeId = v.TypeId
            });

        return result;
    }

    /// <summary>
    /// Метод получает названия резолюций задач по их Id.
    /// </summary>
    /// <param name="resolutionIds">Id резолюций задач.</param>
    /// <returns>Словарь с резолюциями задач.</returns>
    public async Task<IDictionary<int, TaskResolutionOutput>> GetResolutionNamesByResolutionIdsAsync(
        IEnumerable<int> resolutionIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.task_resolutions")
            .WhereIn("resolution_id", resolutionIds)
            .Select("resolution_id", "resolution_name", "position")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();

        var result = (await connection.QueryAsync<TaskResolutionOutput>(sql))
            .ToDictionary(k => k.ResolutionId, v => new TaskResolutionOutput
            {
                ResolutionName = v.ResolutionName,
                ResolutionSysName = v.ResolutionSysName,
                Position = v.Position
            });

        return result;
    }

    /// <inheritdoc />
    public async Task<ProjectTaskEntity> GetTaskDetailsByTaskIdAsync(long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@project_id", projectId);
        parameters.Add("@project_task_id", projectTaskId);

        var query = @"SELECT task_id, task_status_id, author_id, watcher_ids, name, details, created, updated,
                      project_id, project_task_id, resolution_id, tag_ids, task_type_id, executor_id, priority_id 
                      FROM project_management.project_tasks 
                      WHERE project_id = @project_id 
                        AND project_task_id = @project_task_id";

        var result = await connection.QueryFirstOrDefaultAsync<ProjectTaskEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<EpicEntity> GetEpicDetailsByEpicIdAsync(long projectEpicId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@project_id", projectId);
        parameters.Add("@projectEpicId", projectEpicId);

        var query = @"SELECT epic_id,
                           epic_name,
                           epic_description,
                           created_by,
                           created_at,
                           updated_at,
                           updated_by,
                           project_id,
                           initiative_id,
                           date_start,
                           date_end,
                           priority_id,
                           tag_ids,
                           resolution_id,
                           project_epic_id,
                           status_id AS TaskStatusId
                        FROM project_management.epics
                        WHERE project_id = @project_id
                          AND project_epic_id = @projectEpicId";

        var result = await connection.QueryFirstOrDefaultAsync<EpicEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<UserStoryOutput> GetUserStoryDetailsByUserStoryIdAsync(long projectStoryId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@project_id", projectId);
        parameters.Add("@projectStoryId", projectStoryId);

        var query = "SELECT story_id," +
                    "story_name," +
                    "story_description," +
                    "created_by," +
                    "created_at," +
                    "updated_at," +
                    "updated_by," +
                    "project_id," +
                    "story_status_id," +
                    "watcher_ids," +
                    "resolution_id," +
                    "tag_ids," +
                    "epic_id," +
                    "executor_id," +
                    "user_story_task_id," +
                    "status_id AS TaskStatusId " +
                    "FROM project_management.user_stories " +
                    "WHERE project_id = @project_id " +
                    "AND user_story_task_id = @projectStoryId";
        
        var result = await connection.QueryFirstOrDefaultAsync<UserStoryOutput>(query, parameters);

        return result;
    }

    /// <summary>
    /// Метод получает названия приоритетов задач по их Id.
    /// </summary>
    /// <param name="priorityIds">Id приоритетов задач.</param>
    /// <returns>Словарь с приоритетами задач.</returns>
    public async Task<IDictionary<int, TaskPriorityOutput>> GetPriorityNamesByPriorityIdsAsync(
        IEnumerable<int> priorityIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.task_priorities")
            .WhereIn("priority_id", priorityIds)
            .Select("priority_id", "priority_name")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();

        var result = (await connection.QueryAsync<TaskPriorityOutput>(sql))
            .ToDictionary(k => k.PriorityId, v => new TaskPriorityOutput
            {
                PriorityName = v.PriorityName,
                PrioritySysName = v.PrioritySysName
            });

        return result;
    }

    /// <inheritdoc />
    public async Task<long> GetLastProjectTaskIdAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        // Скрипт учитывает все таблицы, где есть айдишники задач в рамках проекта (эпики, истории, задачи, ошибки).
        var query = @"DROP TABLE IF EXISTS temp_max_table;
                    WITH max_task_id AS (SELECT MAX(COALESCE(pt.project_task_id, 0)) AS max_project_project_task_id,
                            MAX(COALESCE(e.project_epic_id, 0))     AS max_epic_project_epic_id,
                            MAX(COALESCE(et.project_task_id, 0))    AS max_epic_tasks_project_task_id,
                            MAX(COALESCE(us.user_story_task_id, 0)) AS max_user_story_task_id 
                     FROM project_management.project_tasks AS pt 
                              LEFT JOIN project_management.epics AS e 
                                        ON pt.project_id = e.project_id 
                              LEFT JOIN project_management.epic_tasks AS et 
                                        ON et.epic_id = e.epic_id 
                              LEFT JOIN project_management.user_stories AS us 
                                        ON pt.project_id = us.project_id 
                     WHERE pt.project_id = @projectId) 
                    SELECT UNNEST(ARRAY [max_task_id.max_project_project_task_id, max_task_id.max_epic_project_epic_id,
                                 max_task_id.max_epic_tasks_project_task_id, max_task_id.max_user_story_task_id]) AS x 
                    INTO TEMP TABLE temp_max_table 
                    FROM max_task_id;

                    SELECT MAX(x) 
                    FROM temp_max_table;";

        var result = await connection.ExecuteScalarAsync<long>(query, parameters);

        return result;
    }

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    public async Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.task_priorities")
            .Select("priority_id", "priority_name", "priority_sys_name")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<TaskPriorityEntity>(sql);

        return result;
    }

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    public async Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.task_types")
            .Select("type_id", "type_name", "type_sys_name")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<TaskTypeEntity>(sql);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectTagEntity>> GetProjectTagsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = @"SELECT tag_id, tag_name, tag_sys_name, tag_description, project_id, object_tag_type 
                      FROM project_management.project_tags 
                      ORDER BY position";

        var result = await connection.QueryAsync<ProjectTagEntity>(query);

        return result;
    }

    /// <inheritdoc />
    public async Task CreateProjectTaskAsync(ProjectTaskEntity task)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@task_status_id", task.TaskStatusId);
        parameters.Add("@author_id", task.AuthorId);
        parameters.Add("@watcher_ids", task.WatcherIds);
        parameters.Add("@name", task.Name);
        parameters.Add("@details", task.Details);
        parameters.Add("@created", DateTime.UtcNow);
        parameters.Add("@project_id", task.ProjectId);
        parameters.Add("@project_task_id", task.ProjectTaskId);
        parameters.Add("@resolution_id", task.ResolutionId);
        parameters.Add("@tag_ids", task.TagIds);
        parameters.Add("@task_type_id", task.TaskTypeId);
        parameters.Add("@executor_id", task.ExecutorId);
        parameters.Add("@priority_id", task.PriorityId);

        var sql = @"INSERT INTO project_management.project_tasks (task_status_id, author_id, watcher_ids, name, details, created, project_id, project_task_id, resolution_id, tag_ids, task_type_id, executor_id, priority_id) 
VALUES (@task_status_id, @author_id, @watcher_ids, @name, @details, @created, @project_id, @project_task_id, @resolution_id, @tag_ids, @task_type_id, @executor_id, @priority_id)";

        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task CreateProjectEpicAsync(EpicEntity epic)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@epicName", epic.EpicName);
        parameters.Add("@epicDescription", epic.EpicDescription);
        parameters.Add("@createdAt", DateTime.UtcNow);
        parameters.Add("@createdBy", epic.CreatedBy);
        parameters.Add("@projectId", epic.ProjectId);
        parameters.Add("@resolutionId", epic.ResolutionId);
        parameters.Add("@tagIds", epic.TagIds);
        parameters.Add("@dateStart", epic.DateStart);
        parameters.Add("@dateEnd", epic.DateEnd);
        parameters.Add("@priorityId", epic.PriorityId);
        parameters.Add("@initiativeId", epic.InitiativeId);
        parameters.Add("@projectEpicId", epic.ProjectEpicId);

        var sql = @"INSERT INTO project_management.epics (epic_name, epic_description, created_by, created_at,
                                      project_id, initiative_id, date_start, date_end, priority_id, tag_ids,
                                      resolution_id, project_epic_id) 
        VALUES (@epicName, @epicDescription, @createdBy, @createdAt, @projectId, @initiativeId, @dateStart, @dateEnd,
                @priorityId, @tagIds, @resolutionId, @projectEpicId)";

        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task CreateProjectUserStoryAsync(UserStoryEntity story)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@storyName", story.StoryName);
        parameters.Add("@storyDescription", story.StoryDescription);
        parameters.Add("@createdAt", DateTime.UtcNow);
        parameters.Add("@createdBy", story.CreatedBy);
        parameters.Add("@projectId", story.ProjectId);
        parameters.Add("@storyStatusId", story.StoryStatusId);
        parameters.Add("@userStoryTaskId", story.TaskStatusId);

        var columns = new StringBuilder(
            @"INSERT INTO project_management.user_stories (story_name, story_description, created_by, created_at,
                                                           project_id, story_status_id, executor_id,
                                                           user_story_task_id");
        var values = new StringBuilder(
            @" VALUES (@storyName, @storyDescription, @createdBy, @createdAt, @projectId, @storyStatusId, @executorId,
                       @userStoryTaskId");
        
        // Если исполнитель не назначался, то автор истории становится исполнителем.
        story.ExecutorId ??= story.CreatedBy;

        parameters.Add("@executorId", story.ExecutorId);

        if (story.EpicId.HasValue)
        {
            parameters.Add("@epicId", story.EpicId);
            columns.Append(", epic_id");
            values.Append(", @epicId");
        }
        
        if (story.TagIds is not null && story.TagIds.Any())
        {
            parameters.Add("@tagIds", story.TagIds);
            columns.Append(", tag_ids");
            values.Append(", @tagIds");
        }
        
        if (story.WatcherIds is not null && story.WatcherIds.Any())
        {
            parameters.Add("@watcherIds", story.WatcherIds);
            columns.Append(", watcher_ids");
            values.Append(", @watcherIds");
        }

        columns.Append(")");
        values.Append(")");

        await connection.ExecuteAsync(string.Concat(columns, values), parameters);
    }

    /// <inheritdoc />
    public async Task<int> GetLastPositionProjectTagAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tags")
            .Where("project_id", projectId)
            .Select("position")
            .AsMax("position");
        var sql = compiler.Compile(query).ToString();
        
        var lastPosition = await connection.ExecuteScalarAsync<int>(sql);
        
        if (lastPosition <= 0)
        {
            // Если позиций пока нету, то начнем с первой.
            return 1;
        }

        return lastPosition;
    }

    /// <inheritdoc />
    public async Task CreateProjectTaskTagAsync(ProjectTagEntity tag)
    {
        tag.ObjectTagTypeValue = ObjectTagTypeEnum.Project;
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@tag_name", tag.TagName);
        parameters.Add("@tag_sys_name", tag.TagSysName);
        parameters.Add("@position", tag.Position, DbType.Int32);
        parameters.Add("@tag_description", tag.TagDescription);
        parameters.Add("@project_id", tag.ProjectId);
        parameters.Add("@object_tag_type", tag.ObjectTagType);

        var query = @"INSERT INTO project_management.project_tags (tag_name, tag_sys_name, position, tag_description, project_id, object_tag_type) 
                      VALUES (@tag_name, @tag_sys_name, @position, @tag_description, @project_id, @object_tag_type)";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<bool> IfProjectHavingProjectTaskIdAsync(long projectId, long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Where("project_task_id", projectTaskId)
            .AsCount();
        var sql = compiler.Compile(query).ToString();
            
        var count = await connection.ExecuteScalarAsync<int>(sql);

        return count > 0;
    }

    /// <inheritdoc />
    public async Task<bool> IfProjectHavingProjectUserStoryIdAsync(long projectId, long projectStoryId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.user_stories")
            .Where("project_id", projectId)
            .Where("user_story_task_id", projectStoryId)
            .AsCount();
        var sql = compiler.Compile(query).ToString();
            
        var count = await connection.ExecuteScalarAsync<int>(sql);

        return count > 0;
    }

    /// <inheritdoc />
    public async Task<bool> IfProjectHavingEpicIdAsync(long projectId, long projectEpicId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.epics")
            .Where("project_id", projectId)
            .Where("project_epic_id", projectEpicId)
            .AsCount();
        var sql = compiler.Compile(query).ToString();
            
        var count = await connection.ExecuteScalarAsync<int>(sql);

        return count > 0;
    }

    /// <inheritdoc />
    public async Task<long> GetProjectTaskStatusIdByProjectIdProjectTaskIdAsync(long projectId, long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Where("project_task_id", projectTaskId)
            .Select("task_status_id");
        var sql = compiler.Compile(query).ToString();
        
        var result = await connection.QueryFirstOrDefaultAsync<long>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<long> GetProjectEpicStatusIdByProjectIdEpicIdIdAsync(long projectId, long projectEpicId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.epics")
            .Where("project_id", projectId)
            .Where("project_epic_id", projectEpicId)
            .Select("status_id");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryFirstOrDefaultAsync<long>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<long> GetProjectUserStoryStatusIdByUserStoryIdAsync(long projectId, long projectStoryId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.user_stories")
            .Where("project_id", projectId)
            .Where("user_story_task_id", projectStoryId)
            .Select("status_id");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryFirstOrDefaultAsync<long>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<long>> GetProjectManagementTransitionIntermediateTemplatesAsync(
        long currentTaskStatusId, TransitionTypeEnum transitionType, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@currentTaskStatusId", currentTaskStatusId);
        parameters.Add("@transitionType", new Enum(transitionType));
        parameters.Add("@templateId", templateId);

        var query = "SELECT tit.to_status_id " +
                    "FROM templates.project_management_transition_intermediate_templates AS tit " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS pmtsit " +
                    "ON tit.to_status_id = pmtsit.status_id " +
                    "INNER JOIN templates.project_management_task_status_templates AS pmtst " +
                    "ON pmtsit.status_id = pmtst.status_id " +
                    "WHERE tit.from_status_id = @currentTaskStatusId " +
                    "AND tit.transition_type = @transitionType " +
                    "AND pmtsit.template_id = @templateId";

        var result = await connection.QueryAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskStatusIntermediateTemplateCompositeOutput>>
        GetTaskStatusIntermediateTemplatesAsync(IEnumerable<long> statusIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@statusIds", statusIds);

        var sql = @"SELECT tsit.status_id, tsit.template_id, tsit.is_custom_status, tst.task_status_id 
                    FROM templates.project_management_task_status_intermediate_templates AS tsit 
                    INNER JOIN templates.project_management_task_status_templates AS tst 
                        ON tsit.status_id = tst.status_id 
                    WHERE tsit.status_id = ANY(@statusIds)";
        
        var result = await connection.QueryAsync<TaskStatusIntermediateTemplateCompositeOutput>(sql, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IDictionary<long, GetTaskStatusTemplateOutput>> GetTaskStatusTemplatesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_templates")
            .Select("status_id", "status_name", "status_sys_name", "position", "task_status_id", "status_description");
        var sql = compiler.Compile(query).ToString();

        var result = (await connection.QueryAsync<GetTaskStatusTemplateOutput>(sql))
            .ToDictionary(k => k.StatusId, v => new GetTaskStatusTemplateOutput
            {
                Position = v.Position,
                StatusName = v.StatusName,
                StatusSysName = v.StatusSysName,
                TaskStatusId = v.TaskStatusId
            });

        return result;
    }

    /// <inheritdoc />
    public async Task<IDictionary<long, UserStatuseTemplateOutput>> GetUserTaskStatusTemplatesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_user_statuse_templates")
            .Select("status_id", "status_name", "status_sys_name", "position", "user_id", "status_description");
        var sql = compiler.Compile(query).ToString();

        var result = (await connection.QueryAsync<UserStatuseTemplateOutput>(sql))
            .ToDictionary(k => k.StatusId, v => new UserStatuseTemplateOutput
            {
                Position = v.Position,
                StatusName = v.StatusName,
                StatusSysName = v.StatusSysName,
                StatusDescription = v.StatusDescription,
                UserId = v.UserId
            });

        return result;
    }

    /// <inheritdoc />
    public async Task ChangeTaskStatusAsync(long projectId, long changeStatusId, long taskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var compiler = new PostgresCompiler();
        var queryUpdateTask = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Where("project_task_id", taskId)
            .AsUpdate(new { task_status_id = changeStatusId });
        var sqlUpdateTask = compiler.Compile(queryUpdateTask).ToString();

        await connection.ExecuteAsync(sqlUpdateTask);
    }

    /// <inheritdoc />
    public async Task ChangeEpicStatusAsync(long projectId, long changeStatusId, long projectEpicId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@changeStatusId", changeStatusId);
        parameters.Add("@projectEpicId", projectEpicId);

        var query = "UPDATE project_management.epics " +
                    "SET status_id = @changeStatusId " +
                    "WHERE project_id = @projectId " +
                    "AND project_epic_id = @projectEpicId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task ChangeStoryStatusAsync(long projectId, long changeStatusId, long projectStoryId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@changeStatusId", changeStatusId);
        parameters.Add("@userStoryTaskId", projectStoryId);

        var query = "UPDATE project_management.user_stories " +
                    "SET status_id = @changeStatusId " +
                    "WHERE project_id = @projectId " +
                    "AND user_story_task_id = @userStoryTaskId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task ChangeSprintStatusAsync(long projectId, long changeStatusId, long sprintStatusId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@changeStatusId", changeStatusId);
        parameters.Add("@sprintStatusId", sprintStatusId);

        var query = "UPDATE project_management.sprints " +
                    "SET status_id = @changeStatusId " +
                    "WHERE project_id = @projectId " +
                    "AND sprint_status_id = @sprintStatusId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<ProjectManagmentTaskStatusTemplateEntity> GetTaskStatusByTaskStatusIdAsync(long taskStatusId,
        int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@task_status_id", taskStatusId);
        parameters.Add("@template_id", templateId);

        var sql = @"SELECT tst.status_id, tst.status_name, tst.status_sys_name, tst.task_status_id 
                    FROM templates.project_management_task_status_intermediate_templates AS tsit 
                    INNER JOIN templates.project_management_task_status_templates AS tst 
                        ON tsit.status_id = tst.status_id 
                    WHERE tst.task_status_id = @task_status_id 
                    AND tsit.template_id = @template_id";

        var result = await connection.QueryFirstOrDefaultAsync<ProjectManagmentTaskStatusTemplateEntity>(
            sql, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task UpdateTaskDetailsAsync(long projectId, long taskId, string changedTaskDetails)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@project_id", projectId);
        parameters.Add("@task_id", taskId);
        parameters.Add("@details", changedTaskDetails);

        var sql = @"UPDATE project_management.project_tasks 
                    SET details = @details 
                    WHERE project_id = @project_id 
                      AND project_task_id = @task_id";
        
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateTaskNameAsync(long projectId, long taskId, string changedTaskName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@project_id", projectId);
        parameters.Add("@task_id", taskId);
        parameters.Add("@name", changedTaskName);

        var sql = @"UPDATE project_management.project_tasks 
                    SET name = @name 
                    WHERE project_id = @project_id 
                      AND project_task_id = @task_id";
        
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task AttachTaskTagAsync(int tagId, long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@tag_id", tagId);
        parameters.Add("@project_task_id", projectTaskId);
        parameters.Add("@project_id", projectId);

        var sql = @"UPDATE project_management.project_tasks 
                    SET tag_ids = ARRAY_APPEND(tag_ids, @tag_id) 
                    WHERE project_task_id = @project_task_id 
                      AND project_id = @project_id";
                      
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task DetachTaskTagAsync(int tagId, long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@tag_id", tagId);
        parameters.Add("@project_task_id", projectTaskId);
        parameters.Add("@project_id", projectId);

        var sql = @"UPDATE project_management.project_tasks 
                    SET tag_ids = ARRAY_REMOVE(tag_ids, @tag_id) 
                    WHERE project_task_id = @project_task_id 
                      AND project_id = @project_id";
                      
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task AttachTaskWatcherAsync(long watcherId, long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@watcher_ids", watcherId);
        parameters.Add("@project_task_id", projectTaskId);
        parameters.Add("@project_id", projectId);

        var sql = @"UPDATE project_management.project_tasks 
                    SET watcher_ids = ARRAY_APPEND(watcher_ids, @watcher_ids) 
                    WHERE project_task_id = @project_task_id 
                      AND project_id = @project_id";
                      
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task DetachTaskWatcherAsync(long watcherId, long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@watcherId", watcherId);
        parameters.Add("@project_task_id", projectTaskId);
        parameters.Add("@project_id", projectId);

        var sql = @"UPDATE project_management.project_tasks 
                    SET watcher_ids = ARRAY_REMOVE(watcher_ids, @watcherId::BIGINT) 
                    WHERE project_task_id = @project_task_id 
                      AND project_id = @project_id";
                      
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateTaskExecutorAsync(long executorId, long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@executor_id", executorId);
        parameters.Add("@project_task_id", projectTaskId);
        parameters.Add("@project_id", projectId);

        var sql = @"UPDATE project_management.project_tasks 
                    SET executor_id = @executor_id 
                    WHERE project_task_id = @project_task_id 
                      AND project_id = @project_id";
                      
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateTaskPriorityAsync(int priorityId, long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@priority_id", priorityId);
        parameters.Add("@project_task_id", projectTaskId);
        parameters.Add("@project_id", projectId);

        var sql = @"UPDATE project_management.project_tasks 
                    SET priority_id = @priority_id 
                    WHERE project_task_id = @project_task_id 
                      AND project_id = @project_id";
                      
        await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task CreateTaskLinkAsync(long taskFromLink, long taskToLink, LinkTypeEnum linkType, long projectId,
        long? childId, long? parentId, long? dependId)
    {
        switch (linkType)
        {
            // Если создается обычная связь.
            case LinkTypeEnum.Link:
                await CreateTaskLinkDefaultAsync(taskFromLink, taskToLink, LinkTypeEnum.Link, projectId);
                break;
            
            // Если создается родительская связь.
            case LinkTypeEnum.Parent:
                await CreateTaskLinkParentAsync(taskFromLink, parentId!.Value, LinkTypeEnum.Parent, projectId);
                break;
            
            // Если создается дочерняя связь.
            case LinkTypeEnum.Child:
                await CreateTaskLinkChildAsync(taskFromLink, childId!.Value, LinkTypeEnum.Child, projectId);
                break;
            
            // Если создается тип связи "зависит от".
            case LinkTypeEnum.Depend:
                await CreateTaskLinkDependAsync(taskFromLink, dependId!.Value, LinkTypeEnum.Depend, projectId);
                break;
            
            default:
                throw new InvalidOperationException($"Недопустимый тип связи {linkType}.");
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectTaskExtendedEntity>> GetProjectTaskByProjectIdTaskIdsAsync(long projectId,
        IEnumerable<long> taskIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@taskIds", taskIds.AsList());
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

        var query = "SELECT t.task_id," +
                             "t.task_status_id," +
                             "t.author_id," +
                             "t.watcher_ids," +
                             "t.name," +
                             "t.details," +
                             "t.created," +
                             "t.updated," +
                             "t.project_id," +
                             "t.project_task_id," +
                             "t.resolution_id," +
                             "t.tag_ids," +
                             "t.task_type_id," +
                             "t.executor_id," +
                             "t.priority_id," +
                             "(SELECT \"ParamValue\"" +
                                       "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                                       "WHERE ps.\"ProjectId\" = @projectId " +
                                       "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                      "FROM project_management.project_tasks AS t " +
                      "WHERE t.project_id = @projectId " + 
                        "AND t.task_id = ANY(@taskIds)";
        
        var result = await connection.QueryAsync<ProjectTaskExtendedEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskLinkExtendedEntity>> GetTaskLinksByProjectIdProjectTaskIdAsync(long projectId,
        long fromTaskId, LinkTypeEnum linkType)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        IEnumerable<TaskLinkExtendedEntity> result;

        if (linkType == LinkTypeEnum.Depend)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@project_id", projectId);
            parameters.Add("@from_task_id", fromTaskId);
            parameters.Add("@link_type", new Enum(linkType));
            parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

            var query = "SELECT link_id," + 
                       "from_task_id," + 
                       "to_task_id," + 
                       "link_type," + 
                       "parent_id," + 
                       "child_id," + 
                       "is_blocked," + 
                       "project_id," + 
                       "blocked_task_id," +
                       "(SELECT \"ParamValue\"" +
                                 "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                                 "WHERE ps.\"ProjectId\" = @project_id " +
                                 "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                      "FROM project_management.task_links " + 
                      "WHERE project_id = @project_id " + 
                        "AND from_task_id = @from_task_id " + 
                        "AND link_type = @link_type";

            result = await connection.QueryAsync<TaskLinkExtendedEntity>(query, parameters);
        }

        else
        {
            var parameters = new DynamicParameters();
            parameters.Add("@project_id", projectId);
            parameters.Add("@from_task_id", fromTaskId);
            parameters.Add("@link_type", new Enum(linkType));
            parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

            var query = "SELECT link_id," + 
                       "from_task_id," + 
                       "to_task_id," + 
                       "link_type," + 
                       "parent_id," + 
                       "child_id," + 
                       "is_blocked," + 
                       "project_id," + 
                       "blocked_task_id, " + 
                       "(SELECT \"ParamValue\"" +
                       "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                       "WHERE ps.\"ProjectId\" = @project_id " +
                       "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                      "FROM project_management.task_links " + 
                      "WHERE project_id = @project_id " + 
                        "AND from_task_id = @from_task_id " + 
                        "AND link_type = @link_type";

            result = await connection.QueryAsync<TaskLinkExtendedEntity>(query, parameters);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AvailableTaskLinkOutput>> GetAvailableTaskLinkAsync(long projectId,
        LinkTypeEnum linkType)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@linkType", new Enum(linkType));
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

        var query = "SELECT DISTINCT (pt.task_id) AS TaskId," +
                                        "pt.name AS TaskName," +
                                        "pt.project_task_id AS ProjectTaskId," +
                                        "pt.task_status_id AS TaskStatusId," +
                                        "pt.executor_id AS ExecutorId," +
                                        "pt.priority_id AS PriorityId," +
                                        "pt.updated AS LastUpdated," +
                                        "pt.created," +
                                        "(SELECT \"ParamValue\"" +
                                                  "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                                                  "WHERE ps.\"ProjectId\" = @projectId " +
                                                  "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix " +
                      "FROM project_management.project_tasks AS pt " + 
                               "LEFT JOIN project_management.task_links AS tl ON pt.task_id = tl.to_task_id " +
                      "WHERE pt.project_id = @projectId " + 
                        "AND pt.task_id NOT IN (SELECT tl.to_task_id " + 
                                               "FROM project_management.task_links " + 
                                               "WHERE tl.link_type = @linkType) " + 
                       "ORDER BY pt.created";

        var result = await connection.QueryAsync<AvailableTaskLinkOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task RemoveTaskLinkAsync(LinkTypeEnum linkType, long removedLinkId, long currentTaskId,
        long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        // Если идет удаление обычной связи.
        if (linkType == LinkTypeEnum.Link)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@from_task_id", currentTaskId);
            parameters.Add("@to_task_id", removedLinkId);
            parameters.Add("@project_id", projectId);
            parameters.Add("@link_type", new Enum(linkType));
            
            var query = @"DELETE 
                          FROM project_management.task_links 
                          WHERE from_task_id = @from_task_id 
                            AND to_task_id = @to_task_id 
                            AND project_id = @project_id 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(query, parameters);
        }
        
        // Если идет удаление родительской связи.
        if (linkType == LinkTypeEnum.Parent)
        {
            // Сначала удаляем запись родителя.
            var parentParameters = new DynamicParameters();
            parentParameters.Add("@parent_id", removedLinkId);
            parentParameters.Add("@from_task_id", currentTaskId);
            parentParameters.Add("@project_id", projectId);
            parentParameters.Add("@link_type", new Enum(LinkTypeEnum.Parent));
            
            var parentQuery = @"DELETE 
                          FROM project_management.task_links 
                          WHERE parent_id = @parent_id 
                            AND from_task_id = @from_task_id 
                            AND project_id = @project_id 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(parentQuery, parentParameters);
            
            // Затем удаляем запись дочки этого родителя.
            var childParameters = new DynamicParameters();
            parentParameters.Add("@child_id", removedLinkId);
            parentParameters.Add("@from_task_id", currentTaskId);
            parentParameters.Add("@project_id", projectId);
            parentParameters.Add("@link_type", new Enum(LinkTypeEnum.Child));
            
            var childQuery = @"DELETE 
                          FROM project_management.task_links 
                          WHERE child_id = @child_id 
                            AND from_task_id = @from_task_id 
                            AND project_id = @project_id 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(childQuery, childParameters);
        }
        
        // Если идет удаление дочерней связи.
        if (linkType == LinkTypeEnum.Child)
        {
            // Сначала удаляем запись дочки.
            var childParameters = new DynamicParameters();
            childParameters.Add("@child_id", removedLinkId);
            childParameters.Add("@from_task_id", currentTaskId);
            childParameters.Add("@project_id", projectId);
            childParameters.Add("@link_type", new Enum(LinkTypeEnum.Child));
            
            var childQuery = @"DELETE 
                          FROM project_management.task_links 
                          WHERE child_id = @child_id 
                            AND from_task_id = @from_task_id 
                            AND project_id = @project_id 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(childQuery, childParameters);
            
            // Затем удаляем запись родителя этой дочки.
            var parentParameters = new DynamicParameters();
            parentParameters.Add("@parent_id", currentTaskId);
            parentParameters.Add("@from_task_id", removedLinkId);
            parentParameters.Add("@project_id", projectId);
            parentParameters.Add("@link_type", new Enum(LinkTypeEnum.Parent));
            
            var parentQuery = @"DELETE 
                          FROM project_management.task_links 
                          WHERE child_id = @child_id 
                            AND from_task_id = @from_task_id 
                            AND project_id = @project_id 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(parentQuery, parentParameters);
        }

        // Если идет удаление зависимой задачи.
        if (linkType == LinkTypeEnum.Depend)
        {
            // Сначала удаляем связь зависит от.
            var firstParameters = new DynamicParameters();
            firstParameters.Add("@from_task_id", currentTaskId);
            firstParameters.Add("@to_task_id", removedLinkId);
            firstParameters.Add("@project_id", projectId);
            firstParameters.Add("@is_blocked", false);
            firstParameters.Add("@link_type", new Enum(LinkTypeEnum.Depend));
            
            var firstQuery = @"DELETE 
                          FROM project_management.task_links 
                          WHERE from_task_id = @from_task_id 
                            AND to_task_id = @to_task_id 
                            AND project_id = @project_id 
                            AND is_blocked = @is_blocked 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(firstQuery, firstParameters);
            
            // Затем удаляем связь блокирует.
            var secondParameters = new DynamicParameters();
            secondParameters.Add("@from_task_id", removedLinkId);
            secondParameters.Add("@blocked_task_id", currentTaskId);
            secondParameters.Add("@project_id", projectId);
            secondParameters.Add("@is_blocked", true);
            secondParameters.Add("@link_type", new Enum(LinkTypeEnum.Depend));
            
            var secondQuery = @"DELETE 
                          FROM project_management.task_links 
                          WHERE from_task_id = @from_task_id 
                            AND blocked_task_id = @blocked_task_id 
                            AND is_blocked = @is_blocked 
                            AND project_id = @project_id 
                            AND link_type = @link_type";
            
            await connection.ExecuteAsync(secondQuery, secondParameters);
        }
    }

    /// <inheritdoc />
    public async Task CreateProjectTaskDocumentsAsync(IEnumerable<ProjectDocumentEntity> documents,
        DocumentTypeEnum documentType, List<string?> mongoDocumentIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var transaction = connection.BeginTransaction();

        var parameters = new List<DynamicParameters>();

        try
        {
            var i = 0;
            
            foreach (var d in documents)
            {
                var tempParameters = new DynamicParameters();
                tempParameters.Add("@documentType", new Enum(documentType));
                tempParameters.Add("@documentName", d.DocumentName);
                tempParameters.Add("@documentExtension", Path.GetExtension(d.DocumentName));
                tempParameters.Add("@created", DateTime.UtcNow);
                tempParameters.Add("@projectId", d.ProjectId);
                tempParameters.Add("@taskId", d.TaskId);
                tempParameters.Add("@userId", d.UserId);
                tempParameters.Add("@mongoDocumentId", mongoDocumentIds[i]);

                parameters.Add(tempParameters);

                i++;
            }

            var query = @"INSERT INTO documents.project_documents (document_type, document_name, document_extension,
                                         created, project_id, task_id, user_id, mongo_document_id) 
                      VALUES (@documentType, @documentName, @documentExtension, @created, @projectId,
                              @taskId, @userId, @mongoDocumentId)";
                              
            await connection.ExecuteAsync(query, parameters);
            
            transaction.Commit();
        }
        
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectDocumentEntity>> GetProjectTaskFilesAsync(long projectId, long taskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@taskId", taskId);

        var query = @"SELECT document_id, document_name, document_extension, mongo_document_id 
                      FROM documents.project_documents 
                      WHERE project_id = @projectId 
                        AND task_id = @taskId";

        var result = await connection.QueryAsync<ProjectDocumentEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<string> GetDocumentNameByDocumentIdAsync(long documentId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@documentId", documentId);

        var query = @"SELECT document_name 
                      FROM documents.project_documents 
                      WHERE document_id = @documentId";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<(long? UserId, string DocumentName, DocumentTypeEnum DocumentType)>>
        GetDocumentNameByDocumentIdsAsync(
            IEnumerable<(long? UserId, long? DocumentId, DocumentTypeEnum DocumentType)> userDocs)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var docs = userDocs.AsList();

        var parameters = new DynamicParameters();
        parameters.Add("@documentIds", docs.Select(x => x.DocumentId).AsList());
        parameters.Add("@userIds", docs.Select(x => x.UserId).AsList());
        parameters.Add("@documentTypes",
            docs.Select(x => System.Enum.Parse<DocumentTypeEnum>(x.DocumentType.ToString())).AsList());

        var query = @"SELECT user_id, document_name, document_type 
                      FROM documents.project_documents 
                      WHERE document_id = ANY(@documentIds) 
                        AND user_id IS NOT NULL 
                        AND user_id = ANY(@userIds) 
                        AND document_type = ANY(@documentTypes)";

        var items = await connection.QueryAsync<(long? UserId, string DocumentName, string DocumentType)>(
            query, parameters);
        var result = new List<(long? UserId, string DocumentName, DocumentTypeEnum DocumentType)>();

        foreach (var item in items)
        {
            result.Add((item.UserId, item.DocumentName,
                System.Enum.Parse<DocumentTypeEnum>(item.DocumentType.ToPascalCaseFromSnakeCase())));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task RemoveDocumentAsync(long documentId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@documentId", documentId);

        var query = @"DELETE 
                      FROM documents.project_documents 
                      WHERE document_id = @documentId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task RemoveDocumentAsync(string? mongoDocumentId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@mongoDocumentId", mongoDocumentId);

        var query = @"DELETE 
                      FROM documents.project_documents 
                      WHERE mongo_document_id = @mongoDocumentId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task FixationProjectViewStrategyAsync(ProjectStrategyEnum strategySysName, long projectId,
        long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@userId", userId);
        parameters.Add("@strategy", new Enum(strategySysName));

        var query = "INSERT INTO settings.project_user_strategy (project_id, user_id, strategy)" +
                    "VALUES (@projectId, @userId, @strategy)" +
                    "ON CONFLICT (project_id, user_id) " +
                    "DO UPDATE SET strategy = @strategy ";
        
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task CreateTaskCommentAsync(long projectTaskId, long projectId, string comment, long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskId", projectTaskId);
        parameters.Add("@comment", comment);
        parameters.Add("@createdBy", userId);

        var query = @"INSERT INTO project_management.task_comments (project_id, project_task_id, comment, created_by) 
                      VALUES (@projectId, @projectTaskId, @comment, @createdBy)";
                      
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectTaskCommentExtendedEntity>> GetTaskCommentsAsync(long projectTaskId,
        long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskId", projectTaskId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);

        var query = "SELECT tc.comment_id," +
                    " tc.project_id," +
                    " tc.project_task_id," +
                    " tc.created_at," +
                    " tc.updated_at," +
                    " tc.comment," +
                    " tc.created_by," +
                    " tc.updated_by, " +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "pi.\"FirstName\" || ' ' || pi.\"LastName\" || ' ' || (pi.\"Patronymic\") AS UserName" +
                    " FROM project_management.task_comments AS tc " +
                    " INNER JOIN \"Profile\".\"ProfilesInfo\" AS pi ON tc.created_by = pi.\"UserId\"" +
                    "WHERE tc.project_id = @projectId " +
                    "AND tc.project_task_id = @projectTaskId " +
                    "ORDER BY tc.created_at DESC";

        var result = await connection.QueryAsync<ProjectTaskCommentExtendedEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task UpdateTaskCommentAsync(long projectTaskId, long projectId, long commentId, string comment,
        long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskId", projectTaskId);
        parameters.Add("@commentId", commentId);
        parameters.Add("@comment", comment);
        parameters.Add("@updatedAt", DateTime.UtcNow);
        parameters.Add("@userId", userId);

        var query = @"UPDATE project_management.task_comments 
                      SET comment = @comment,
                          updated_at = @updatedAt,
                          updated_by = @userId 
                          WHERE project_id = @projectId 
                            AND project_task_id = @projectTaskId 
                            AND comment_id = @commentId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task DeleteTaskCommentAsync(long commentId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@commentId", commentId);
        
        var query = @"DELETE FROM project_management.task_comments 
                      WHERE comment_id = @commentId";
                      
        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<long?> GetUserAvatarDocumentIdByUserIdAsync(long userId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        parameters.Add("@projectId", projectId);
        parameters.Add("@documentType", new Enum(DocumentTypeEnum.ProjectUserAvatar));

        var query = @"SELECT document_id 
                      FROM documents.project_documents 
                      WHERE project_id = @projectId 
                        AND user_id = @userId 
                        AND document_type = @documentType 
                        LIMIT 1";

        var result = await connection.QuerySingleOrDefaultAsync<long?>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<(long? UserId, long? DocumentId, DocumentTypeEnum DocumentType)>>
        GetUserAvatarDocumentIdByUserIdsAsync(IEnumerable<long> userIds, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userIds", userIds.AsList());
        parameters.Add("@projectId", projectId);
        parameters.Add("@documentType", new Enum(DocumentTypeEnum.ProjectUserAvatar));

        var query = @"SELECT user_id, document_id, document_type 
                      FROM documents.project_documents 
                      WHERE project_id = @projectId 
                        AND user_id = ANY(@userIds) 
                        AND document_type = @documentType";

        var items = await connection.QueryAsync<(long? UserId, long? DocumentId, string DocumentType)>(
            query, parameters);
        var result = new List<(long? UserId, long? DocumentId, DocumentTypeEnum DocumentType)>();

        foreach (var item in items)
        {
            result.Add((item.UserId, item.DocumentId,
                System.Enum.Parse<DocumentTypeEnum>(item.DocumentType.ToPascalCaseFromSnakeCase())));
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<EpicEntity>> GetEpicsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT epic_id," +
                    "epic_name," +
                    "epic_description," +
                    "created_by," +
                    "created_at," +
                    "updated_at," +
                    "updated_by, " +
                    "project_id " +
                    "FROM project_management.epics " + 
                    "WHERE project_id = @projectId";

        var result = await connection.QueryAsync<EpicEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<EpicEntity>> GetAvailableEpicsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        
        var query = "SELECT epic_id," +
                    "epic_name " +
                    "FROM project_management.epics " + 
                    "WHERE project_id = @projectId";

        var result = await connection.QueryAsync<EpicEntity>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task IncludeTaskEpicAsync(long epicId, IEnumerable<long> projectTaskIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new List<DynamicParameters>();

        foreach (var p in projectTaskIds)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@epicId", epicId);
            tempParameters.Add("@projectTaskIds", p);
            
            parameters.Add(tempParameters);
        }

        var query = @"INSERT INTO project_management.epic_tasks (project_task_id, epic_id) 
                      VALUES (@projectTaskIds, @epicId)";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task<AvailableEpicOutput> GetTaskEpicAsync(long projectId, long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var epicTaskParameters = new DynamicParameters();
        epicTaskParameters.Add("@projectTaskId", projectTaskId);

        var epicTaskQuery = @"SELECT epic_id 
                              FROM project_management.epic_tasks 
                              WHERE project_task_id = @projectTaskId";

        var epicIdResult = await connection.QuerySingleOrDefaultAsync<long?>(epicTaskQuery, epicTaskParameters);

        if (epicIdResult is null)
        {
            return null;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@epicId", epicIdResult);
        parameters.Add("@projectId", projectId);

        var query = @"SELECT epic_id, epic_name 
                      FROM project_management.epics 
                      WHERE epic_id = @epicId 
                        AND project_id = @projectId";
        
        var result = await connection.QuerySingleOrDefaultAsync<AvailableEpicOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserStoryStatusEntity>> GetUserStoryStatusesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = @"SELECT status_id, status_name, status_sys_name 
                      FROM project_management.user_story_statuses";
        
        var result = await connection.QueryAsync<UserStoryStatusEntity>(query);

        return result;
    }

    /// <inheritdoc/>
    public async Task<long> PlaningSprintAsync(PlaningSprintInput planingSprintInput, long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        // Получаем последний Id спринта в рамках проекта, перед созданим нового.
        var lastProjectSprintIdParameters = new DynamicParameters();
        lastProjectSprintIdParameters.Add("@projectId", planingSprintInput.ProjectId);

        var lastProjectSprintIdQuery = "SELECT MAX (project_sprint_id) " +
                                       "FROM project_management.sprints " +
                                       "WHERE project_id = @projectId";
        
        var lastProjectSprintId = await connection.ExecuteScalarAsync<long>(lastProjectSprintIdQuery,
            lastProjectSprintIdParameters);

        var parameters = new DynamicParameters();
        parameters.Add("@dateStart", planingSprintInput.DateStart);
        parameters.Add("@dateEnd", planingSprintInput.DateEnd);
        parameters.Add("@sprintGoal", planingSprintInput.SprintDescription);
        parameters.Add("@sprintStatusId", planingSprintInput.SprintStatus!.Value);
        parameters.Add("@projectId", planingSprintInput.ProjectId);
        parameters.Add("@sprintName", planingSprintInput.SprintName);
        parameters.Add("@projectSprintId", ++lastProjectSprintId);
        parameters.Add("@createdBy", userId);
        parameters.Add("@createdAt", DateTime.UtcNow);
        parameters.Add("@updatedAt", DateTime.UtcNow);

        if (!planingSprintInput.ExecutorId.HasValue)
        {
            parameters.Add("@executorId", userId);
        }
        
        else
        {
            parameters.Add("@executorId", planingSprintInput.ExecutorId.Value);
        }

        string query;

        // Если передали наблюдателей, то учитываем это в запросе.
        if (planingSprintInput.WatcherIds is not null && planingSprintInput.WatcherIds.Any())
        {
            parameters.Add("@watcherIds", planingSprintInput.WatcherIds.AsList());
            
            query = @"INSERT INTO project_management.sprints (date_start, date_end, sprint_goal, sprint_status_id,
                                        project_id, sprint_name, project_sprint_id, created_by, executor_id,
                                        created_at, updated_at, watcher_ids) 
                      VALUES (@dateStart, @dateEnd, @sprintGoal, @sprintStatusId, @projectId, @sprintName,
                              @projectSprintId, @createdBy, @executorId, @createdAt, @updatedAt, @watcherIds) 
                      RETURNING project_sprint_id";
        }

        else
        {
            query = @"INSERT INTO project_management.sprints (date_start, date_end, sprint_goal, sprint_status_id,
                                        project_id, sprint_name, project_sprint_id, created_by, executor_id,
                                        created_at, updated_at) 
                      VALUES (@dateStart, @dateEnd, @sprintGoal, @sprintStatusId, @projectId, @sprintName,
                              @projectSprintId, @createdBy, @executorId, @createdAt, @updatedAt) 
                      RETURNING project_sprint_id";
        }

        var result = await connection.ExecuteScalarAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectIdAsync(long projectTaskId,
        long projectId, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@projectTaskId", projectTaskId);
        parameters.Add("@templateId", templateId);

        var query = "SELECT t.task_id," +
                    "t.name," +
                    "t.project_id," +
                    "t.project_task_id," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "t.task_status_id, " +
                    "t.executor_id," +
                    "t.task_type_id," +
                    "(SELECT CASE WHEN t.task_type_id = 1 THEN 'Задача' " +
                    "WHEN t.task_type_id = 2 THEN 'Ошибка' END) AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = t.executor_id) AS ExecutorName, " +
                    "(SELECT tpmtst.status_name " +
                    "FROM templates.project_management_task_status_templates AS tpmtst " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS tpmtsit " +
                    "ON tpmtst.task_status_id = tpmtsit.status_id " +
                    "WHERE t.task_status_id = tpmtst.task_status_id " +
                    "AND tpmtsit.template_id = @templateId " +
                    "AND NOT tpmtst.is_system_status) AS StatusName " +
                    "FROM project_management.project_tasks AS t " +
                    "WHERE t.project_id = @projectId " +
                    "AND t.project_task_id = @projectTaskId " +
                    "AND t.project_task_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)" +
                    "UNION " +
                    "SELECT e.epic_id," +
                    "e.epic_name AS name," +
                    "e.project_id," +
                    "e.project_epic_id  AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "e.status_id AS TaskStatusId, " +
                    "e.created_by AS ExecutorId, " +
                    "4 AS TaskTypeId, " +
                    "\'Эпик\' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = e.created_by) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.epic_statuses " +
                    "WHERE status_id = e.status_id) AS StatusName " +
                    "FROM project_management.epics AS e " +
                    "INNER JOIN project_management.epic_statuses AS es " +
                    "ON e.status_id = es.status_id " +
                    "WHERE e.project_id = @projectId " +
                    "AND e.project_epic_id = @projectTaskId " +
                    "AND e.project_epic_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)" +
                    "UNION " +
                    "SELECT us.story_id," +
                    "us.story_name AS name," +
                    "us.project_id," +
                    "us.user_story_task_id AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "us.status_id AS TaskStatusId, " +
                    "us.executor_id, " +
                    "3 AS TaskTypeId, " +
                    "'История/Требование' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = us.executor_id) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = us.status_id) AS StatusName " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "WHERE us.project_id = @projectId " +
                    "AND us.user_story_task_id = @projectTaskId " +
                    "AND us.user_story_task_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)";

        var result = await connection.QueryAsync<SearchAgileObjectOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectNameAsync(string taskName,
        long projectId, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@taskName", taskName);
        parameters.Add("@templateId", templateId);

        var query = "SELECT t.task_id," +
                    "t.name," +
                    "t.project_id," +
                    "t.project_task_id," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "t.task_status_id, " +
                    "t.executor_id, " +
                    "t.task_type_id, " +
                    "(SELECT CASE WHEN t.task_type_id = 1 THEN 'Задача' " +
                    "WHEN t.task_type_id = 2 THEN 'Ошибка' END) AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = t.executor_id) AS ExecutorName, " +
                    "(SELECT tpmtst.status_name " +
                    "FROM templates.project_management_task_status_templates AS tpmtst " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS tpmtsit " +
                    "ON tpmtst.task_status_id = tpmtsit.status_id " +
                    "WHERE t.task_status_id = tpmtst.task_status_id " +
                    "AND tpmtsit.template_id = @templateId " +
                    "AND NOT tpmtst.is_system_status) AS StatusName " +
                    "FROM project_management.project_tasks AS t " +
                    "WHERE t.project_id = @projectId " +
                    "AND t.name ILIKE @taskName || '%' " +
                    "AND t.project_task_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)" +
                    "UNION " +
                    "SELECT e.epic_id," +
                    "e.epic_name AS name," +
                    "e.project_id," +
                    "e.project_epic_id  AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "e.status_id AS TaskStatusId, " +
                    "e.created_by AS ExecutorId, " +
                    "4 AS TaskTypeId, " +
                    "\'Эпик\' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = e.created_by) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.epic_statuses " +
                    "WHERE status_id = e.status_id) AS StatusName " +
                    "FROM project_management.epics AS e " +
                    "INNER JOIN project_management.epic_statuses AS es " +
                    "ON e.status_id = es.status_id " +
                    "WHERE e.project_id = @projectId " +
                    "AND e.epic_name ILIKE @taskName || '%' " +
                    "UNION " +
                    "SELECT us.story_id," +
                    "us.story_name AS name," +
                    "us.project_id," +
                    "us.user_story_task_id AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "us.status_id AS TaskStatusId, " +
                    "us.executor_id, " +
                    "3 AS TaskTypeId, " +
                    "\'История/Требование\' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = us.executor_id) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = us.status_id) AS StatusName " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "WHERE us.project_id = @projectId " +
                    "AND us.story_name ILIKE @taskName || '%'" +
                    "AND us.user_story_task_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)";

        var result = await connection.QueryAsync<SearchAgileObjectOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectDescriptionAsync(
        string taskDescription, long projectId, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@taskDescription", taskDescription);
        parameters.Add("@templateId", templateId);

        var query = "SELECT t.task_id," +
                    "t.name," +
                    "t.project_id," +
                    "t.project_task_id," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "t.task_status_id, " +
                    "t.executor_id, " +
                    "t.task_type_id, " +
                    "(SELECT CASE WHEN t.task_type_id = 1 THEN 'Задача' " +
                    "WHEN t.task_type_id = 2 THEN 'Ошибка' END) AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = t.executor_id) AS ExecutorName, " +
                    "(SELECT tpmtst.status_name " +
                    "FROM templates.project_management_task_status_templates AS tpmtst " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS tpmtsit " +
                    "ON tpmtst.task_status_id = tpmtsit.status_id " +
                    "WHERE t.task_status_id = tpmtst.task_status_id " +
                    "AND tpmtsit.template_id = @templateId " +
                    "AND NOT tpmtst.is_system_status) AS StatusName " +
                    "FROM project_management.project_tasks AS t " +
                    "WHERE t.project_id = @projectId " +
                    "AND t.details ILIKE '%' || @taskDescription || '%' " +
                    "AND t.project_task_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)" +
                    "UNION " +
                    "SELECT e.epic_id," +
                    "e.epic_name AS name," +
                    "e.project_id," +
                    "e.project_epic_id  AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "e.status_id AS TaskStatusId, " +
                    "e.created_by AS ExecutorId, " +
                    "4 AS TaskTypeId, " +
                    "\'Эпик\' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = e.created_by) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.epic_statuses " +
                    "WHERE status_id = e.status_id) AS StatusName " +
                    "FROM project_management.epics AS e " +
                    "INNER JOIN project_management.epic_statuses AS es " +
                    "ON e.status_id = es.status_id " +
                    "WHERE e.project_id = @projectId " +
                    "AND e.epic_description ILIKE '%' || @taskDescription || '%' " +
                    "UNION " +
                    "SELECT us.story_id," +
                    "us.story_name AS name," +
                    "us.project_id," +
                    "us.user_story_task_id AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "us.status_id AS TaskStatusId, " +
                    "us.executor_id, " +
                    "3 AS TaskTypeId, " +
                    "\'История/Требование\' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = us.executor_id) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = us.status_id) AS StatusName " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "WHERE us.project_id = @projectId " +
                    "AND us.story_description ILIKE '%' || @taskDescription || '%'" +
                    "AND us.user_story_task_id NOT IN (SELECT project_task_id " +
                    "FROM project_management.epic_tasks)";

        var result = await connection.QueryAsync<SearchAgileObjectOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task IncludeProjectTaskSprintAsync(IEnumerable<long> projectTaskIds, long sprintId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var projectTaskIdsItems = projectTaskIds.AsList();
        
        var parameters = new List<DynamicParameters>(projectTaskIdsItems.Count);

        foreach (var id in projectTaskIdsItems)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@sprintId", sprintId);
            tempParameters.Add("@projectTaskIds", id);
            
            parameters.Add(tempParameters);
        }

        var query = @"INSERT INTO project_management.sprint_tasks (sprint_id, project_task_id) 
                      VALUES (@sprintId, @projectTaskIds)";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task<EpicTaskResult> GetEpicTasksAsync(long projectId, long epicId, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@epicId", epicId);
        parameters.Add("@prefix", GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX);
        parameters.Add("@templateId", templateId);

        var result = new EpicTaskResult
        {
            EpicTasks = new List<EpicTaskOutput>()
        };
        
        var query = "DROP TABLE IF EXISTS temp_epic_task_ids; " +
                    "SELECT project_task_id " +
                    "INTO TEMP TABLE temp_epic_task_ids " +
                    "FROM project_management.epic_tasks " +
                    "WHERE epic_id = @epicId; " +
                    "SELECT t.task_id," +
                    "t.name," +
                    "t.project_id," +
                    "t.project_task_id," +
                    "(SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "t.task_status_id, " +
                    "t.executor_id, " +
                    "t.task_type_id, " +
                    "(SELECT CASE WHEN t.task_type_id = 1 THEN 'Задача' " +
                    "WHEN t.task_type_id = 2 THEN 'Ошибка' END) AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = t.executor_id) AS ExecutorName, " +
                    "(SELECT tpmtst.status_name " +
                    "FROM templates.project_management_task_status_templates AS tpmtst " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS tpmtsit " +
                    "ON tpmtst.status_id = tpmtsit.status_id " +
                    "WHERE t.task_status_id = tpmtst.task_status_id " +
                    "AND tpmtsit.template_id = @templateId " +
                    "AND NOT tpmtst.is_system_status) AS StatusName " +
                    "FROM project_management.project_tasks AS t " +
                    "WHERE t.project_id = @projectId " +
                    "AND t.project_task_id IN (SELECT project_task_id FROM temp_epic_task_ids) " +
                    "UNION " +
                    "SELECT us.story_id," +
                    "us.story_name AS name," +
                    "us.project_id," +
                    "us.user_story_task_id AS project_task_id," +
                    "(SELECT \"ParamValue\"" +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" AS ps " +
                    "WHERE ps.\"ProjectId\" = @projectId " +
                    "AND ps.\"ParamKey\" = @prefix) AS TaskIdPrefix, " +
                    "us.status_id AS TaskStatusId, " +
                    "us.executor_id, " +
                    "3 AS TaskTypeId, " +
                    "\'История/Требование\' AS TaskTypeName, " +
                    "(SELECT \"FirstName\" || ' ' || \"LastName\" || ' ' || (COALESCE(\"Patronymic\", '')) " +
                    "FROM \"Profile\".\"ProfilesInfo\"" + "WHERE \"UserId\" = us.executor_id) AS ExecutorName, " +
                    "(SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = us.status_id) AS StatusName " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "WHERE us.project_id = @projectId " +
                    "AND us.user_story_task_id IN (SELECT project_task_id FROM temp_epic_task_ids)";

        result.EpicTasks = await connection.QueryAsync<EpicTaskOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> GetEpicStatusNameByEpicStatusIdAsync(int statusId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@statusId", statusId);

        var query = "SELECT status_name " +
                    "FROM project_management.epic_statuses " +
                    "WHERE status_id = @statusId";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> GetUserStoryStatusNameByStoryStatusIdAsync(int statusId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@statusId", statusId);

        var query = "SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = @statusId";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<EpicStatusOutput>> GetEpicStatusesAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var query = @"SELECT status_id, status_name, status_sys_name 
                      FROM project_management.epic_statuses";

        var result = await connection.QueryAsync<EpicStatusOutput>(query);

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> IfExistsProjectTaskAsync(string taskName, int taskType, long projectId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@taskName", taskName);
        parameters.Add("@taskType", taskType);
        parameters.Add("@projectId", projectId);

        var query = "SELECT EXISTS (SELECT task_id " +
                    "FROM project_management.project_tasks " +
                    "WHERE task_type_id = @taskType " +
                    "AND project_id = @projectId " +
                    "AND name = @taskName)";
        
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<string?> GetProjectUserStrategyAsync(long projectId, long userId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@userId", userId);

        var query = "SELECT strategy " +
                    "FROM settings.project_user_strategy " +
                    "WHERE project_id = @projectId " +
                    "AND user_id = @userId";
        
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var result = await connection.QueryFirstOrDefaultAsync<string?>(query, parameters);

        result ??= "kn";

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WorkSpaceOutput>> GetWorkSpacesAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT up.\"ProjectId\", " +
                    "COALESCE(up.\"ProjectManagementName\", 'Проект без названия') AS ProjectManagementName, " +
                    "pw.workspace_id " +
                    "FROM \"Projects\".\"UserProjects\" AS up " +
                    "INNER JOIN project_management.workspaces AS pw " +
                    "ON up.\"ProjectId\" = pw.project_id " +
                    "INNER JOIN \"Teams\".\"ProjectsTeams\" AS pt " +
                    "ON up.\"ProjectId\" = pt.\"ProjectId\" " +
                    "INNER JOIN \"Teams\".\"ProjectsTeamsMembers\" AS ptm " +
                    "ON pt.\"TeamId\" = ptm.\"TeamId\" " +
                    "WHERE ptm.\"UserId\" = @userId " +
                    "ORDER BY pw.workspace_id DESC";

        var result = await connection.QueryAsync<WorkSpaceOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<TaskSprintOutput> GetSprintTaskAsync(long projectId, long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskId", projectTaskId);

        var query = "SELECT s.sprint_id," +
                    "s.sprint_name " +
                    "FROM project_management.sprints AS s " +
                    "INNER JOIN project_management.sprint_tasks AS st " +
                    "ON s.sprint_id = st.sprint_id " +
                    "WHERE s.project_id = @projectId " +
                    "AND st.project_task_id = @projectTaskId";

        var result = await connection.QueryFirstOrDefaultAsync<TaskSprintOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskSprintOutput>> GetAvailableProjectSprintsAsync(long projectId,
        long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskId", projectTaskId);
        
        var query = "WITH cte_sprint_tasks AS ( " +
                    "SELECT project_task_id " +
                    "FROM project_management.sprint_tasks) " +
                    "SELECT sprint_id, sprint_name " +
                    "FROM project_management.sprints " +
                    "WHERE project_id = @projectId " +
                    "AND @projectTaskId <> ANY (SELECT * FROM cte_sprint_tasks)";

        var result = await connection.QueryAsync<TaskSprintOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task InsertOrUpdateTaskSprintAsync(long sprintId, long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectTaskId", projectTaskId);
        parameters.Add("@sprintId", sprintId);

        var queryIfExists = "SELECT EXISTS (SELECT sprint_task_id " +
                            "FROM project_management.sprint_tasks " +
                            "WHERE project_task_id = @projectTaskId)";
        var queryCheck = await connection.ExecuteScalarAsync<bool>(queryIfExists, parameters);

        // Если уже есть такая задача в каком то спринте, то обновляем ей спринт.
        if (queryCheck)
        {
            var updateQuery = "UPDATE project_management.sprint_tasks " +
                              "SET sprint_id = @sprintId " +
                              "WHERE project_task_id = @projectTaskId";
            await connection.ExecuteAsync(updateQuery, parameters);
        }

        // Иначе включаем задачу в спринт.
        else
        {
            var insertQuery = "INSERT INTO project_management.sprint_tasks (sprint_id, project_task_id) " +
                              "VALUES (@sprintId, @projectTaskId)";
            await connection.ExecuteAsync(insertQuery, parameters);
        }
    }
    
    /// <inheritdoc/>
    public async Task<long> CreateCompanyAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "INSERT INTO project_management.organizations (created_by) " +
                    "VALUES (@userId) " +
                    "RETURNING organization_id";

        var result = await connection.ExecuteScalarAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> IfExistsCompanyByOwnerIdAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT EXISTS (SELECT organization_id " +
                    "FROM project_management.organizations " +
                    "WHERE created_by = @userId)";

        var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<long> CreateCompanyWorkSpaceAsync(long projectId, long companyId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var lastWorkSpaceIdQuery = "SELECT workspace_id " +
                                   "FROM project_management.workspaces " +
                                   "ORDER BY workspace_id DESC " +
                                   "LIMIT 1";
                                   
        var lastWorkSpaceId = await connection.ExecuteScalarAsync<long>(lastWorkSpaceIdQuery);
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@companyId", companyId);
        parameters.Add("@workspaceId", ++lastWorkSpaceId);

        var query = "INSERT INTO project_management.workspaces (workspace_id, project_id, organization_id) " +
                    "VALUES (@workspaceId, @projectId, @companyId) " +
                    "RETURNING workspace_id";

        var result = await connection.ExecuteScalarAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task AddProjectWorkSpaceAsync(long projectId, long companyId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var projectWorkSpaceParameters = new DynamicParameters();
        projectWorkSpaceParameters.Add("@projectId", projectId);
        projectWorkSpaceParameters.Add("@companyId", companyId);
        projectWorkSpaceParameters.Add("@isActive", true);

        var projectWorkSpaceQuery = "INSERT INTO project_management.organization_projects (organization_id, project_id, is_active) " +
                    "VALUES (@companyId, @projectId, @isActive)";

        await connection.ExecuteAsync(projectWorkSpaceQuery, projectWorkSpaceParameters);

        var parametersWorkSpace = new DynamicParameters();
        parametersWorkSpace.Add("@projectId", projectId);
        parametersWorkSpace.Add("@companyId", companyId);
        
        var WorkSpaceQuery = "INSERT INTO project_management.workspaces (project_id, organization_id) " +
                             "VALUES (@projectId, @companyId)";
        
        await connection.ExecuteAsync(WorkSpaceQuery, parametersWorkSpace);
    }

    /// <inheritdoc/>
    public async Task AddCompanyMemberAsync(long companyId, long userId, string? memberRole)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        parameters.Add("@companyId", companyId);
        parameters.Add("@memberRole", memberRole);

        string query;

        if (!string.IsNullOrWhiteSpace(memberRole))
        {
            query = "INSERT INTO project_management.organization_members (organization_id, member_role, member_id) " +
                    "VALUES (@companyId, @memberRole, @userId)";
        }

        else
        {
            query = "INSERT INTO project_management.organization_members (organization_id, member_id) " +
                    "VALUES (@companyId, @userId)";
        }

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task<long> GetCompanyIdByOwnerIdAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        
        var query = "SELECT organization_id " +
                    "FROM project_management.organizations " +
                    "WHERE created_by = @userId";

        var result = await connection.ExecuteScalarAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckCompanyOwnerByUserIdAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT EXISTS (SELECT organization_id " +
                    "FROM project_management.organizations " +
                    "WHERE created_by = @userId " +
                    "LIMIT 1)";

        var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task AddProjectWorkSpaceMemberAsync(long projectId, long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var memberCompanyParameters = new DynamicParameters();
        memberCompanyParameters.Add("@userId", userId);
        memberCompanyParameters.Add("@projectId", projectId);

        var companyMemberQuery = "SELECT om.organization_id " +
                           "FROM project_management.organization_members AS om " +
                           "INNER JOIN project_management.organization_projects AS op " +
                           "ON om.organization_id = op.organization_id " +
                           "WHERE om.member_id = @userId " +
                           "AND op.project_id = @projectId";
        
        var organizationId = await connection.QueryFirstOrDefaultAsync<long?>(companyMemberQuery,
            memberCompanyParameters);

        // Если пользователя нет в компании, значит сначала добавляем пользователя в компанию.
        if (organizationId is null)
        {
            // Находим Id организации по проекту.
            var projectCompanyParameters = new DynamicParameters();
            projectCompanyParameters.Add("@projectId", projectId);

            var projectCompanyQuery = "SELECT organization_id " +
                                      "FROM project_management.organization_projects " +
                                      "WHERE project_id = @projectId";

            organizationId = await connection.QueryFirstOrDefaultAsync<long?>(projectCompanyQuery,
                projectCompanyParameters);

            if (!organizationId.HasValue)
            {
                throw new InvalidOperationException("Не удалось получить Id компании по проекту. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}.");
            }
        }
        
        // Добавляем сотрудника в компанию.
        var memberParameters = new DynamicParameters();
        memberParameters.Add("@organizationId", organizationId);
            
        // TODO: Когда роль будет передавать с фронта, то записывать ее тут.
        memberParameters.Add("@memberRole", "Участник");
        memberParameters.Add("@userId", userId);

        var memberQuery = "INSERT INTO project_management.organization_members (organization_id, member_role," +
                          " member_id) " +
                          "VALUES (@organizationId, @memberRole, @userId)";

        await connection.ExecuteAsync(memberQuery, memberParameters);
    }

    /// <inheritdoc/>
    public async Task<bool> IfEpicAvailableStatusAsync(long changeStatus)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@changeStatus", changeStatus);

        var query = "SELECT EXISTS (" +
                    "SELECT status_id " +
                    "FROM project_management.epic_statuses " +
                    "WHERE status_id = @changeStatus)";

        var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

        return result;
    }
    
    /// <inheritdoc/>
    public async Task<bool> IfStoryAvailableStatusAsync(long changeStatus)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@changeStatus", changeStatus);

        var query = "SELECT EXISTS (" +
                    "SELECT status_id " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = @changeStatus)";

        var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task RemoveProjectTasksAsync(long projectId, TaskDetailTypeEnum taskType, List<long>? taskIds = null,
        List<ProjectManagementDocumentFile>? documents = null, List<long>? epicIds = null, List<long>? storyIds = null)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        
        try
        {
            // Удаляем связи, в которых участвуют удаляемые задачи.
            var removeParameters = new List<DynamicParameters>();

            // Заполняем параметры Id задач и ошибок.
            if (taskIds is not null
                && taskIds.Count > 0
                && taskType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
            {
                foreach (var x in taskIds)
                {
                    var tempParameters = new DynamicParameters();
                    tempParameters.Add("@taskIds", x);
                    tempParameters.Add("@projectId", projectId);
                    
                    removeParameters.Add(tempParameters);
                }
            }
            
            // Заполняем параметры Id эпиков.
            if (epicIds is not null
                && epicIds.Count > 0
                && taskType is TaskDetailTypeEnum.Epic)
            {
                foreach (var x in epicIds)
                {
                    var tempParameters = new DynamicParameters();
                    tempParameters.Add("@taskIds", x);
                    tempParameters.Add("@projectId", projectId);
                    
                    removeParameters.Add(tempParameters);
                }
            }
            
            // Заполняем параметры Id историй.
            if (storyIds is not null
                && storyIds.Count > 0
                && taskType is TaskDetailTypeEnum.Story)
            {
                foreach (var x in storyIds)
                {
                    var tempParameters = new DynamicParameters();
                    tempParameters.Add("@taskIds", x);
                    tempParameters.Add("@projectId", projectId);
                    
                    removeParameters.Add(tempParameters);
                }
            }

            var taskLinksQuery = "DELETE FROM project_management.task_links " +
                                 "WHERE project_id = @projectId " +
                                 "AND (parent_id = ANY(@taskIds) " +
                                 "OR child_id = ANY(@taskIds) " +
                                 "OR blocked_task_id = ANY(@taskIds) " +
                                 "OR from_task_id = ANY(@taskIds) " +
                                 "OR to_task_id = ANY(@taskIds))";

            await connection.ExecuteAsync(taskLinksQuery, removeParameters);
            
            // Удаляем все комментарии удаляемых задач.
            var removeTaskCommentsQuery = "DELETE FROM project_management.task_comments " +
                                          "WHERE project_id = @projectId " +
                                          "AND project_task_id = ANY(@taskIds)";
            
            await connection.ExecuteAsync(removeTaskCommentsQuery, removeParameters);

            // Удаляем файлы задачи.
            // Файлы комментариев удалятся автоматически, так как там есть каскад на удаление.
            if (documents is not null && documents.Count > 0)
            {
                var documentIds = documents.Where(x => x.DocumentId.HasValue).Select(x => x.DocumentId!.Value)
                    .AsList();
                
                var removeTaskFilesParameters = new DynamicParameters();
                removeTaskFilesParameters.Add("@taskIds", taskIds);
                removeTaskFilesParameters.Add("@projectId", projectId);
                removeTaskFilesParameters.Add("@documentIds", documentIds);
                
                var removeTaskFilesQuery = "DELETE FROM documents.project_documents " +
                                           "WHERE project_id = @projectId " +
                                           "AND task_id = ANY(@taskIds) " +
                                           "AND document_id = ANY(@documentIds)";
                
                await connection.ExecuteAsync(removeTaskFilesQuery, removeTaskFilesParameters);
            }

            // Удаляем задачу - для типов задачи "Задача", "Ошибка".
            if (taskType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
            {
                var removeTaskParameters = new DynamicParameters();
                removeTaskParameters.Add("@taskIds", taskIds);
                removeTaskParameters.Add("@projectId", projectId);

                var removeTaskQuery = "DELETE FROM project_management.project_tasks " +
                                      "WHERE project_id = @projectId " +
                                      "AND project_task_id = ANY(@taskIds)";
            
                await connection.ExecuteAsync(removeTaskQuery, removeTaskParameters);
            }
            
            // Удаляем эпик.
            // Задачи из эпика удалятся автоматически, так как там есть каскад на удаление.
            if (taskType is TaskDetailTypeEnum.Epic)
            {
                var removeEpicParameters = new DynamicParameters();
                removeEpicParameters.Add("@taskIds", epicIds);
                removeEpicParameters.Add("@projectId", projectId);

                var removeEpicQuery = "DELETE FROM project_management.epics " +
                                      "WHERE project_id = @projectId " +
                                      "AND project_epic_id = ANY(@taskIds)";
            
                await connection.ExecuteAsync(removeEpicQuery, removeEpicParameters);
            }
            
            // Удаляем историю.
            // Эпики истории удалятся автоматически, так как там есть каскад на удаление.
            if (taskType is TaskDetailTypeEnum.Story)
            {
                var removeStoryParameters = new DynamicParameters();
                removeStoryParameters.Add("@taskIds", storyIds);
                removeStoryParameters.Add("@projectId", projectId);

                var removeStoryQuery = "DELETE FROM project_management.user_stories " +
                                       "WHERE project_id = @projectId " +
                                       "AND user_story_task_id = ANY(@taskIds)";
            
                await connection.ExecuteAsync(removeStoryQuery, removeStoryParameters);
            }

            transaction.Commit();
        }
        
        catch
        {
            transaction.Rollback();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectManagementDocumentFile>> IfProjectTaskExistFileAsync(long projectId,
        IEnumerable<long> taskIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@taskIds", taskIds.AsList());

        var query = "SELECT document_id, project_id, task_id, user_id " +
                    "FROM documents.project_documents " +
                    "WHERE project_id = @projectId " +
                    "AND task_id = ANY(@taskIds)";

        var result = await connection.QueryAsync<ProjectManagementDocumentFile>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<long> GetCompanyIdByProjectIdAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        
        var query = "SELECT organization_id " +
                    "FROM project_management.organization_projects " +
                    "WHERE project_id = @projectId";

        var result = await connection.QueryFirstOrDefaultAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<long> GetEpicIdByProjectEpicIdAsync(long projectId, long projectEpicId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectEpicId", projectEpicId);

        var query = "SELECT epic_id " +
                    "FROM project_management.epics " +
                    "WHERE project_epic_id = @projectEpicId " +
                    "AND project_id = @projectId";
        
        var result = await connection.ExecuteScalarAsync<long>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<TaskDetailTypeEnum> GetTaskTypeByProjectIdProjectTaskIdAsync(long projectId, long projectTaskId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskId", projectTaskId);

        // Ищем среди задач и ошибок.
        var searchTaskQuery = "SELECT task_type_id " +
                              "FROM project_management.project_tasks " +
                              "WHERE project_id = @projectId " +
                              "AND project_task_id = @projectTaskId";

        var searchTaskResult = await connection.QueryFirstOrDefaultAsync<long?>(searchTaskQuery, parameters);

        if (searchTaskResult.HasValue)
        {
            if (searchTaskResult == 1)
            {
                return TaskDetailTypeEnum.Error;
            }
            
            if (searchTaskResult == 3)
            {
                return TaskDetailTypeEnum.Task;
            }

            throw new InvalidOperationException(
                "Поиск типа задачи по типу задачи или ошибке сработал, но что то пошло не так. " +
                $"SearchTaskResult: {searchTaskResult}.");
        }
        
        // Ищем в эпиках.
        var searchEpicQuery = "SELECT epic_id " +
                              "FROM project_management.epics " +
                              "WHERE project_id = @projectId " +
                              "AND project_epic_id = @projectTaskId";

        var searchEpicResult = await connection.QueryFirstOrDefaultAsync<long?>(searchEpicQuery, parameters);
        
        if (searchEpicResult.HasValue)
        {
            return TaskDetailTypeEnum.Epic;
        }
        
        // Ищем в историях.
        var searchStoryQuery = "SELECT story_status_id " +
                              "FROM project_management.user_stories " +
                              "WHERE project_id = @projectId " +
                              "AND user_story_task_id = @projectTaskId";

        var searchStoryResult = await connection.QueryFirstOrDefaultAsync<long?>(searchStoryQuery, parameters);
        
        if (searchStoryResult.HasValue)
        {
            return TaskDetailTypeEnum.Story;
        }
        
        // Ищем в спринтах.
        var searchSprintQuery = "SELECT sprint_status_id " +
                               "FROM project_management.sprints " +
                               "WHERE project_id = @projectId " +
                               "AND project_sprint_id = @projectTaskId";

        var searchSprintResult = await connection.QueryFirstOrDefaultAsync<long?>(searchSprintQuery, parameters);
        
        if (searchSprintResult.HasValue)
        {
            return TaskDetailTypeEnum.Sprint;
        }

        return TaskDetailTypeEnum.Undefined;
    }
    
    /// <inheritdoc />
    public async Task<IDictionary<long, ProjectTaskTypeOutput>> GetProjectTaskStatusesAsync(long projectId,
        IEnumerable<long> projectTaskIds, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskIds", projectTaskIds.AsList());
        parameters.Add("@templateId", templateId);

        var query = "SELECT t.project_task_id, " +
                    "(SELECT tpmtst.status_name " +
                    "FROM templates.project_management_task_status_templates AS tpmtst " +
                    "INNER JOIN templates.project_management_task_status_intermediate_templates AS tpmtsit " +
                    "ON tpmtst.status_id = tpmtsit.status_id " +
                    "WHERE t.task_status_id = tpmtst.status_id " +
                    "AND tpmtsit.template_id = @templateId " +
                    "AND NOT tpmtst.is_system_status) AS TaskStatusName " +
                    "FROM project_management.project_tasks AS t " +
                    "WHERE t.project_task_id = ANY(@projectTaskIds) " +
                    "AND t.project_id = @projectId";

        var result = (await connection.QueryAsync<ProjectTaskTypeOutput>(query, parameters))
            .ToDictionary(k => k.ProjectTaskId, v => v);

        return result;
    }

    /// <inheritdoc />
    public async Task<IDictionary<long, ProjectTaskTypeOutput>> GetProjectStoryStatusesAsync(long projectId,
        IEnumerable<long> projectTaskIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@projectTaskIds", projectTaskIds.AsList());

        var query = "SELECT us.user_story_task_id AS project_task_id, " +
                    "(SELECT status_name " +
                    "FROM project_management.user_story_statuses " +
                    "WHERE status_id = us.status_id) AS TaskStatusName " +
                    "FROM project_management.user_stories AS us " +
                    "INNER JOIN project_management.user_story_statuses AS uss " +
                    "ON us.status_id = uss.status_id " +
                    "WHERE us.project_id = @projectId " +
                    "AND us.user_story_task_id = ANY(@projectTaskIds)";

        var result = (await connection.QueryAsync<ProjectTaskTypeOutput>(query, parameters))
            .ToDictionary(k => k.ProjectTaskId, v => v);

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает обычную связь между задачами.
    /// </summary>
    /// <param name="taskFromLink">Id задачи, от которой исходит связь.</param>
    /// <param name="taskToLink">Id задачи, которую связывают.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="projectId">Id проекта.</param>
    private async Task CreateTaskLinkDefaultAsync(long taskFromLink, long taskToLink, LinkTypeEnum linkType,
        long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Параметры текущей задачи.
            var currentParameters = new DynamicParameters();
            currentParameters.Add("@from_task_id", taskFromLink);
            currentParameters.Add("@to_task_id", taskToLink);
            currentParameters.Add("@link_type", new Enum(linkType));
            currentParameters.Add("@is_blocked", false);
            currentParameters.Add("@project_id", projectId);
            
            // Параметры задачи, с которой устанавливается связь.
            var otherParameters = new DynamicParameters();
            otherParameters.Add("@from_task_id", taskFromLink);
            otherParameters.Add("@to_task_id", taskToLink);
            otherParameters.Add("@link_type", new Enum(linkType));
            otherParameters.Add("@is_blocked", false);
            otherParameters.Add("@project_id", projectId);

            var query = @"INSERT INTO project_management.task_links (
                                           from_task_id, to_task_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @to_task_id, @link_type, @is_blocked, @project_id)";

            await connection.ExecuteAsync(query, currentParameters);
            await connection.ExecuteAsync(query, otherParameters);

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }
    
    /// <summary>
    /// Метод создает родительскую связь между задачами.
    /// </summary>
    /// <param name="taskFromLink">Id задачи, от которой исходит связь.</param>
    /// <param name="parentId">Id родительской задачи.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="projectId">Id проекта.</param>
    private async Task CreateTaskLinkParentAsync(long taskFromLink, long parentId, LinkTypeEnum linkType,
        long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Параметры текущей задачи.
            var currentParameters = new DynamicParameters();
            currentParameters.Add("@from_task_id", taskFromLink);
            
            // Проставляем родителя текущей задаче.
            currentParameters.Add("@parent_id", parentId);
            currentParameters.Add("@link_type", new Enum(linkType));
            currentParameters.Add("@is_blocked", false);
            currentParameters.Add("@project_id", projectId);
            
            var queryFirst = @"INSERT INTO project_management.task_links (
                                           from_task_id, parent_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @parent_id, @link_type, @is_blocked, @project_id)";
            
            // Параметры задачи, с которой устанавливается связь.
            var otherParameters = new DynamicParameters();
            
            // Родительская задача становится текущей.
            otherParameters.Add("@from_task_id", parentId);
            
            // Текущая задача становится дочкой для родителя.
            otherParameters.Add("@child_id", taskFromLink);
            
            // Для родителя текущая задача становится дочкой.
            otherParameters.Add("@link_type", new Enum(LinkTypeEnum.Child));
            otherParameters.Add("@is_blocked", false);
            otherParameters.Add("@project_id", projectId);
            
            var querySecond = @"INSERT INTO project_management.task_links (
                                           from_task_id, child_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @child_id, @link_type, @is_blocked, @project_id)";

            await connection.ExecuteAsync(queryFirst, currentParameters);
            await connection.ExecuteAsync(querySecond, otherParameters);

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }
    
    /// <summary>
    /// Метод создает дочернюю связь между задачами.
    /// </summary>
    /// <param name="taskFromLink">Id задачи, от которой исходит связь.</param>
    /// <param name="childId">Id дочерней задачи.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="projectId">Id проекта.</param>
    private async Task CreateTaskLinkChildAsync(long taskFromLink, long childId, LinkTypeEnum linkType,
        long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Параметры текущей задачи.
            var currentParameters = new DynamicParameters();
            currentParameters.Add("@from_task_id", taskFromLink);
            
            // Проставляем дочку текущей задаче.
            currentParameters.Add("@child_id", childId);
            currentParameters.Add("@link_type", new Enum(linkType));
            currentParameters.Add("@is_blocked", false);
            currentParameters.Add("@project_id", projectId);
            
            var queryFirst = @"INSERT INTO project_management.task_links (
                                           from_task_id, child_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @child_id, @link_type, @is_blocked, @project_id)";
            
            // Параметры задачи, с которой устанавливается связь.
            var otherParameters = new DynamicParameters();
            
            // Дочка становится текущей.
            otherParameters.Add("@from_task_id", childId);
            
            // Текущая задача становится родителем для дочки.
            otherParameters.Add("@parent_id", taskFromLink);
            
            // Для дочки текущая задача становится родителем.
            otherParameters.Add("@link_type", new Enum(LinkTypeEnum.Parent));
            otherParameters.Add("@is_blocked", false);
            otherParameters.Add("@project_id", projectId);
            
            var querySecond = @"INSERT INTO project_management.task_links (
                                           from_task_id, parent_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @parent_id, @link_type, @is_blocked, @project_id)";

            await connection.ExecuteAsync(queryFirst, currentParameters);
            await connection.ExecuteAsync(querySecond, otherParameters);

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }
    
    /// <summary>
    /// Метод создает зависимую связь между задачами.
    /// </summary>
    /// <param name="taskFromLink">Id задачи, от которой исходит связь.</param>
    /// <param name="childId">Id дочерней задачи.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="projectId">Id проекта.</param>
    private async Task CreateTaskLinkDependAsync(long taskFromLink, long dependId, LinkTypeEnum linkType,
        long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Параметры текущей задачи.
            var currentParameters = new DynamicParameters();
            currentParameters.Add("@from_task_id", taskFromLink);
            currentParameters.Add("@to_task_id", dependId);
            currentParameters.Add("@link_type", new Enum(linkType));
            currentParameters.Add("@is_blocked", false);
            currentParameters.Add("@project_id", projectId);
            
            var queryFirst = @"INSERT INTO project_management.task_links (
                                           from_task_id, to_task_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @to_task_id, @link_type, @is_blocked, @project_id)";
            
            // Параметры задачи, с которой устанавливается связь.
            var otherParameters = new DynamicParameters();
            
            // Текущей задачей становится зависимая задача (то есть текущая от нее зависит).
            otherParameters.Add("@from_task_id", dependId);
            
            // Указываем, какую задачу блокирует.
            otherParameters.Add("@blocked_task_id", taskFromLink);
            otherParameters.Add("@link_type", new Enum(linkType));
            
            // Задача, от которой зависит текущая становится для нее блокирующей.
            otherParameters.Add("@is_blocked", true);
            otherParameters.Add("@project_id", projectId);
            
            var querySecond = @"INSERT INTO project_management.task_links (
                                           from_task_id, blocked_task_id, link_type, is_blocked, project_id) 
                      VALUES (@from_task_id, @blocked_task_id, @link_type, @is_blocked, @project_id)";

            await connection.ExecuteAsync(queryFirst, currentParameters);
            await connection.ExecuteAsync(querySecond, otherParameters);

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    #endregion
}