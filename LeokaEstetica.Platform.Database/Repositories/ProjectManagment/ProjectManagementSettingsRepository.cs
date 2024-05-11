using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория настроек проекта.
/// </summary>
internal sealed class ProjectManagementSettingsRepository : BaseRepository, IProjectManagementSettingsRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public ProjectManagementSettingsRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SprintDurationSetting>> GetProjectSprintsDurationSettingsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT setting_id, name, sys_name, tooltip, selected, disabled, project_id " +
                    "FROM settings.sprint_duration_settings " +
                    "WHERE project_id = @projectId " +
                    "ORDER BY setting_id";

        var result = await connection.QueryAsync<SprintDurationSetting>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SprintMoveNotCompletedTaskSetting>>
        GetProjectSprintsMoveNotCompletedTasksSettingsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT setting_id, name, sys_name, tooltip, selected, disabled, project_id " +
                    "FROM settings.move_not_completed_tasks_settings " +
                    "WHERE project_id = @projectId " +
                    "ORDER BY setting_id";

        var result = await connection.QueryAsync<SprintMoveNotCompletedTaskSetting>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task UpdateProjectSprintsDurationSettingsAsync(long projectId, bool isSettingSelected, string sysName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@isSettingSelected", isSettingSelected);
        parameters.Add("@sysName", sysName);

        var query = "UPDATE settings.sprint_duration_settings " +
                    "SET selected = @isSettingSelected " +
                    "WHERE project_id = @projectId " +
                    "AND sys_name = @sysName";

        await connection.ExecuteAsync(query, parameters);
        
        // Проставляем обратные настройки не выбранным.
        var reverseParameters = new DynamicParameters();
        reverseParameters.Add("@projectId", projectId);
        reverseParameters.Add("@isSettingSelected", !isSettingSelected);
        reverseParameters.Add("@sysName", sysName);

        var reverseQuery = "UPDATE settings.sprint_duration_settings " +
                         "SET selected = @isSettingSelected " +
                         "WHERE project_id = @projectId " +
                         "AND sys_name <> @sysName";

        await connection.ExecuteAsync(reverseQuery, reverseParameters);
    }

    /// <inheritdoc/>
    public async Task UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(long projectId, bool isSettingSelected,
        string sysName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@isSettingSelected", isSettingSelected);
        parameters.Add("@sysName", sysName);

        var query = "UPDATE settings.move_not_completed_tasks_settings " +
                    "SET selected = @isSettingSelected " +
                    "WHERE project_id = @projectId " +
                    "AND sys_name = @sysName";

        await connection.ExecuteAsync(query, parameters);
        
        // Проставляем обратные настройки не выбранным.
        var reverseParameters = new DynamicParameters();
        reverseParameters.Add("@projectId", projectId);
        reverseParameters.Add("@isSettingSelected", !isSettingSelected);
        reverseParameters.Add("@sysName", sysName);

        var reverseQuery = "UPDATE settings.move_not_completed_tasks_settings " +
                    "SET selected = @isSettingSelected " +
                    "WHERE project_id = @projectId " +
                    "AND sys_name <> @sysName";

        await connection.ExecuteAsync(reverseQuery, reverseParameters);
    }
}