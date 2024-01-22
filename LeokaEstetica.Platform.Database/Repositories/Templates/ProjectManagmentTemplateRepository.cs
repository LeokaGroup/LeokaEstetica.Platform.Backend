using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Models.Entities.Template;
using SqlKata;
using SqlKata.Compilers;

namespace LeokaEstetica.Platform.Database.Repositories.Templates;

/// <summary>
/// Класс реализует методы репозитория шаблонов модуля УП.
/// </summary>
internal sealed class ProjectManagmentTemplateRepository : BaseRepository, IProjectManagmentTemplateRepository
{
    /// <summary>
    /// Конструктор.
    /// <param name="pgContext">Датаконтекст.</param>
    /// </summary>
    public ProjectManagmentTemplateRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<int?> GetProjectTemplateIdAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        
        // TODO: Изменить схему и название таблицы, когда сделаем ренейм в БД.
        var query = new Query("Projects.UserProjects")
            .Where("ProjectId", projectId)
            .Select("TemplateId");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryFirstOrDefaultAsync<int?>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<long>> GetTemplateStatusIdsAsync(int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_intermediate_templates")
            .Where("template_id", templateId)
            .Select("status_id");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<long>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetTaskTemplateStatusesAsync(
        IEnumerable<long> statusIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_templates")
            .WhereIn("status_id", statusIds)
            .Select("status_id", "status_name", "status_sys_name", "position", "task_status_id", "status_description")
            .OrderBy("position");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryAsync<ProjectManagmentTaskStatusTemplateEntity>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<ProjectManagmentTaskStatusTemplateEntity> GetProjectManagementStatusBySysNameAsync(
        string associationStatusSysName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_templates")
            .Where("status_sys_name", associationStatusSysName)
            .Select("status_id", "status_name", "status_sys_name", "position", "task_status_id", "status_description");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryFirstOrDefaultAsync<ProjectManagmentTaskStatusTemplateEntity>(sql);

        return result;
    }

    /// <inheritdoc />
    public async Task<long> CreateProjectManagmentTaskStatusTemplateAsync(
        ProjectManagementUserStatuseTemplateEntity statusTemplate)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@status_name", statusTemplate.StatusName);
        parameters.Add("@status_sys_name", statusTemplate.StatusSysName);
        parameters.Add("@position", statusTemplate.Position);
        parameters.Add("@user_id", statusTemplate.UserId);
        parameters.Add("@status_description", statusTemplate.StatusDescription);

        var query = @"INSERT INTO templates.project_management_user_statuse_templates (status_name, status_sys_name, 
                                                                 position, user_id, status_description)
                      VALUES (@status_name, @status_sys_name, @position, @user_id, @status_description)";

        var insertedStatusId = await connection.ExecuteAsync(query, parameters);

        return insertedStatusId;
    }

    /// <inheritdoc />
    public async Task<int> GetLastPositionUserStatusTemplateAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_user_statuse_templates")
            .Where("user_id", userId)
            .Select("position")
            .AsMax("position");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.ExecuteScalarAsync<int>(sql);

        if (result <= 0)
        {
            // Если позиций пока нету, то начнем с первой.
            return 1;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task CreateProjectManagmentTaskStatusIntermediateTemplateAsync(long statusId, int templateId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@status_id", statusId);
        parameters.Add("@template_id", templateId);
        parameters.Add("@template_id", true);

        var query = @"INSERT INTO templates.project_management_task_status_intermediate_templates
                      (status_id, template_id, template_id)
                       VALUES (@status_id, @template_id, @template_id)";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <summary>
    /// Метод получает название статуса по TaskStatusId.
    /// </summary>
    /// <param name="taskStatusId">Id статуса задачи.</param>
    /// <returns>Название статуса.</returns>
    public async Task<string> GetStatusNameByTaskStatusIdAsync(int taskStatusId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        var compiler = new PostgresCompiler();
        var query = new Query("templates.project_management_task_status_templates")
            .Where("task_status_id", taskStatusId)
            .Select("status_name");
        var sql = compiler.Compile(query).ToString();

        var result = await connection.QueryFirstOrDefaultAsync<string>(sql);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}