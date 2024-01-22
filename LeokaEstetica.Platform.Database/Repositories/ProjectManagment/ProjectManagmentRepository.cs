using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using SqlKata;
using SqlKata.Compilers;

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
    public async Task<IEnumerable<ProjectManagmentHeaderEntity>> GetHeaderItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var query = @"SELECT header_id,
                             item_name,
                             item_url,
                             position,
                             header_type,
                             items,
                             has_items,
                             is_disabled,
                             control_type,
                             destination
                             FROM project_management.header
                             ORDER BY position";

        var result = await connection.QueryAsync<ProjectManagmentHeaderEntity>(query);

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
                      tst.task_status_id 
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
    public async Task<IEnumerable<ProjectTaskEntity>> GetProjectTasksAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Select("task_id", "task_status_id", "author_id", "watcher_ids", "name", "details", "created", "updated",
                "project_id", "project_task_id", "resolution_id", "tag_ids", "task_type_id", "executor_id",
                "priority_id")
            .OrderBy("project_id");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<ProjectTaskEntity>(sql);

        return result;
    }

    /// <summary>
    /// Метод получает названия тегов (меток) пользователя задач по их Id.
    /// </summary>
    /// <param name="tagIds">Id тегов (меток) задач.</param>
    /// <returns>Словарь с тегами (метками) задач.</returns>
    public async Task<IDictionary<int, UserTaskTagOutput>> GetTagNamesByTagIdsAsync(IEnumerable<int> tagIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.user_task_tags")
            .WhereIn("tag_id", tagIds)
            .Select("tag_id", "tag_name", "position")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();
        
        var result = (await connection.QueryAsync<UserTaskTagOutput>(sql))
            .ToDictionary(k => k.TagId, v => new UserTaskTagOutput
            {
                TagName = v.TagName,
                TagSysName = v.TagSysName,
                TagDescription = v.TagDescription,
                UserId = v.UserId,
                TagId = v.TagId
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
                TypeSysName = v.TypeSysName
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

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    public async Task<ProjectTaskEntity> GetTaskDetailsByTaskIdAsync(long projectTaskId, long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Where("project_task_id", projectTaskId);
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryFirstOrDefaultAsync<ProjectTaskEntity>(sql);

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

    /// <summary>
    /// Метод получает последний Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Последний Id задачи в рамках проекта.</returns>
    public async Task<long> GetLastProjectTaskIdAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Select("task_id")
            .AsMax("task_id");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.ExecuteScalarAsync<long>(sql);

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

    /// <summary>
    /// Метод получает список тегов пользователя для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    public async Task<IEnumerable<UserTaskTagEntity>> GetTaskTagsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.user_task_tags")
            .Select("tag_id", "tag_name", "tag_sys_name", "user_id", "tag_description")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<UserTaskTagEntity>(sql);

        return result;
    }

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="task">Задача для создания.</param>
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
    public async Task<int> GetLastPositionUserTaskTagAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("project_management.user_task_tags")
            .Where("user_id", userId)
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
    public async Task CreateUserTaskTagAsync(UserTaskTagEntity tag)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@tag_name", tag.TagName);
        parameters.Add("@tag_sys_name", tag.TagSysName);
        parameters.Add("@tag_description", tag.TagDescription);
        parameters.Add("@position", tag.Position);
        parameters.Add("@user_id", tag.UserId);

        var query = @"INSERT INTO project_management.user_task_tags (tag_name, tag_sys_name, position, user_id, 
                                               tag_description) 
VALUES (@tag_name, @tag_sys_name, @tag_description, @position, @user_id)";

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
    public async Task<IEnumerable<long>> GetProjectManagementTransitionIntermediateTemplatesAsync(
        long currentTaskStatusId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_transition_intermediate_templates")
            .Where("from_status_id", currentTaskStatusId)
            .Select("to_status_id");
        var sql = compiler.Compile(query).ToString();
        
        var result = await connection.QueryAsync<long>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskStatusIntermediateTemplateEntity>>
        GetTaskStatusIntermediateTemplatesAsync(IEnumerable<long> statusIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_intermediate_templates")
            .WhereIn("status_id", statusIds)
            .Select("status_id", "template_id", "is_custom_status");
        var sql = compiler.Compile(query).ToString();
        
        var result = await connection.QueryAsync<ProjectManagmentTaskStatusIntermediateTemplateEntity>(sql);

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
        var queryGetTask = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Where("task_id", taskId)
            .AsCount();
        var sqlGetTask = compiler.Compile(queryGetTask).ToString();
        var task = await connection.ExecuteScalarAsync<long?>(sqlGetTask);

        if (task is null)
        {
            throw new InvalidOperationException($"Задача с Id: {taskId} не найдена у проекта: {projectId}.");
        }

        var queryUpdateTask = new Query("project_management.project_tasks")
            .Where("project_id", projectId)
            .Where("task_id", taskId)
            .AsUpdate(new { task_status_id = changeStatusId });
        var sqlUpdateTask = compiler.Compile(queryUpdateTask).ToString();

        await connection.ExecuteAsync(sqlUpdateTask);
    }

    #endregion

    #region Приватные методы.

    #endregion
}