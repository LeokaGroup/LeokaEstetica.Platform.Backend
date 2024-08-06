using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
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

    #region Публичные методы.

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
    public async Task<IEnumerable<SprintMoveNotCompletedTaskSetting>?>
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

    /// <inheritdoc/>
    public async Task<SprintDurationSetting?> GetProjectSprintDurationSettingsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT sys_name " +
                    "FROM settings.sprint_duration_settings " +
                    "WHERE selected IS TRUE " +
                    "AND project_id = @projectId";

        var result = await connection.QueryFirstOrDefaultAsync<SprintDurationSetting>(query, parameters);

        return result;
    }

    /// <inheritdoc/>
    public async Task ConfigureProjectScrumSettingsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        // Заводим настройки длительности спринтов проекта.
        var parametersSprintDuration = new DynamicParameters();
        parametersSprintDuration.Add("@name", "2 недели");
        parametersSprintDuration.Add("@sysName", "TwoWeek");
        parametersSprintDuration.Add("@tooltip", "(длительность по умолчанию)");
        parametersSprintDuration.Add("@selected", true);
        parametersSprintDuration.Add("@disabled", false);
        parametersSprintDuration.Add("@projectId", projectId);

        var querySprintDuration = "INSERT INTO settings.sprint_duration_settings (name, sys_name, tooltip, selected," +
                                  " disabled, project_id) " +
                                  "VALUES (@name, @sysName, @tooltip, @selected, @disabled, @projectId)";
        
        await connection.ExecuteAsync(querySprintDuration, parametersSprintDuration);
        
        // Заводим настройки перемещения нерешенных задач спринтов проекта.
        var parametersSprintNotCompletedTasks = new DynamicParameters();
        parametersSprintNotCompletedTasks.Add("@name", "Бэклог проекта");
        parametersSprintNotCompletedTasks.Add("@sysName", "Backlog");
        parametersSprintNotCompletedTasks.Add("@tooltip", "(выбрано по умолчанию)");
        parametersSprintNotCompletedTasks.Add("@selected", true);
        parametersSprintNotCompletedTasks.Add("@disabled", false);
        parametersSprintNotCompletedTasks.Add("@projectId", projectId);

        var querySprinsNotCompletedTasks = "INSERT INTO settings.move_not_completed_tasks_settings (name, sys_name," +
                                           " tooltip, selected, disabled, project_id) " +
                                           "VALUES (@name, @sysName, @tooltip, @selected, @disabled, @projectId)";

        await connection.ExecuteAsync(querySprinsNotCompletedTasks, parametersSprintNotCompletedTasks);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectSettingUserOutput>> GetCompanyProjectUsersAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "WITH cte_company_project_users AS (" +
                    "SELECT pi.\"UserId\", " +
                    "pi.\"LastName\", " +
                    "pi.\"FirstName\", " +
                    "pi.\"Patronymic\" AS \"SecondName\", " +
                    "u.\"Email\", " +
                    "TO_CHAR(u.\"LastAutorization\", 'DD.MM.YYYY HH24:MI') AS LastAutorization, " +
                    "(CASE WHEN u.\"UserId\" = (SELECT up.\"UserId\" " +
                    "FROM \"Projects\".\"UserProjects\" AS up " +
                    "WHERE up.\"ProjectId\" = @projectId " +
                    "LIMIT 1) THEN TRUE " +
                    "ELSE FALSE END) AS IsOwner, " +
                    "COALESCE(om.member_role, 'Участник') AS Role " +
                    "FROM project_management.organization_projects AS op " +
                    "INNER JOIN project_management.organization_members AS om " +
                    "ON op.organization_id = om.organization_id " +
                    "INNER JOIN \"Profile\".\"ProfilesInfo\" AS pi " +
                    "ON om.member_id = \"UserId\" " +
                    "INNER JOIN dbo.\"Users\" AS u " +
                    "ON pi.\"UserId\" = u.\"UserId\" " +
                    "WHERE op.project_id = @projectId " +
                    "AND op.is_active " +
                    "AND pi.\"UserId\" = ANY (SELECT ptm.\"UserId\" " +
                    "FROM \"Teams\".\"ProjectsTeams\" AS pt " +
                    "INNER JOIN \"Teams\".\"ProjectsTeamsMembers\" AS ptm " +
                    "ON pt.\"TeamId\" = ptm.\"TeamId\" " +
                    "WHERE pt.\"ProjectId\" = @projectId))" +
                    "SELECT * " +
                    "FROM cte_company_project_users " +
                    "UNION " +
                    "SELECT pi.\"UserId\", " +
                    "pi.\"LastName\", " +
                    "pi.\"FirstName\", " +
                    "pi.\"Patronymic\" AS \"SecondName\", " +
                    "u.\"Email\", " +
                    "TO_CHAR(u.\"LastAutorization\", 'DD.MM.YYYY HH24:MI') AS LastAutorization, " +
                    "(CASE WHEN u.\"UserId\" = (SELECT up.\"UserId\" " +
                    "FROM \"Projects\".\"UserProjects\" AS up " +
                    "WHERE up.\"ProjectId\" = @projectId " +
                    "LIMIT 1) THEN TRUE " +
                    "ELSE FALSE END) AS IsOwner, " +
                    "COALESCE(om.member_role, 'Участник') AS Role " +
                    "FROM project_management.organization_projects AS op " +
                    "INNER JOIN project_management.organization_members AS om " +
                    "ON op.organization_id = om.organization_id " +
                    "INNER JOIN \"Profile\".\"ProfilesInfo\" AS pi " +
                    "ON om.member_id = \"UserId\" " +
                    "INNER JOIN dbo.\"Users\" AS u " +
                    "ON pi.\"UserId\" = u.\"UserId\" " +
                    "WHERE op.project_id = @projectId " +
                    "AND op.is_active " +
                    "AND pi.\"UserId\" IN (SELECT pi.\"UserId\" " +
                    "FROM cte_company_project_users) " +
                    "AND pi.\"UserId\" = ANY (SELECT ptm.\"UserId\" " +
                    "FROM \"Teams\".\"ProjectsTeams\" AS pt " +
                    "INNER JOIN \"Teams\".\"ProjectsTeamsMembers\" AS ptm " +
                    "ON pt.\"TeamId\" = ptm.\"TeamId\" " +
                    "WHERE pt.\"ProjectId\" = @projectId)";

        var result = await connection.QueryAsync<ProjectSettingUserOutput>(query, parameters);

        return result;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<ProjectInviteOutput>> GetProjectInvitesAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@inviteType", NotificationTypeEnum.ProjectInvite.ToString());
        
        var query = "SELECT n.\"NotificationId\", " +
                    "u.\"Email\", " +
                    "n.\"UserId\", " +
                    "TO_CHAR(n.\"Created\", 'DD.MM.YYYY HH24:MI') AS \"CreatedAt\" " +
                    "FROM \"Notifications\".\"Notifications\" AS n " +
                    "INNER JOIN dbo.\"Users\" AS u " +
                    "ON n.\"UserId\" = u.\"UserId\" " +
                    "WHERE n.\"ProjectId\" = @projectId " +
                    "AND n.\"IsNeedAccepted\" " +
                    "AND NOT n.\"Approved\" " +
                    "AND NOT n.\"Rejected\" " +
                    "AND n.\"NotificationType\" = @inviteType " +
                    "AND n.\"NotificationType\" NOT IN ('ApproveInviteProject', 'RejectInviteProject')";

        var result = await connection.QueryAsync<ProjectInviteOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task CancelProjectInviteAsync(long notificationId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@notificationId", notificationId);
        
        var query = "DELETE FROM \"Notifications\".\"Notifications\" " +
                    "WHERE \"NotificationId\" = @notificationId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc/>
    public async Task AddCompanyMemberRolesAsync(long organizationId, long memberId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@organizationId", organizationId);
        parameters.Add("@memberId", memberId);

        // По дефолту проставляем все роли участнику.
        var query = @"INSERT INTO roles.project_member_roles (role_id, project_member_id, organization_id, is_enabled)
                        VALUES (1, @memberId, @organizationId, TRUE),
                               (2, @memberId, @organizationId, TRUE),
                               (3, @memberId, @organizationId, TRUE),
                               (4, @memberId, @organizationId, TRUE),
                               (5, @memberId, @organizationId, TRUE),
                               (6, @memberId, @organizationId, TRUE),
                               (7, @memberId, @organizationId, TRUE),
                               (8, @memberId, @organizationId, TRUE),
                               (9, @memberId, @organizationId, TRUE),
                               (10, @memberId, @organizationId, TRUE),
                               (11, @memberId, @organizationId, TRUE),
                               (12, @memberId, @organizationId, TRUE),
                               (13, @memberId, @organizationId, TRUE),
                               (14, @memberId, @organizationId, TRUE),
                               (15, @memberId, @organizationId, TRUE),
                               (16, @memberId, @organizationId, TRUE),
                               (17, @memberId, @organizationId, TRUE),
                               (18, @memberId, @organizationId, TRUE),
                               (19, @memberId, @organizationId, TRUE),
                               (20, @memberId, @organizationId, TRUE),
                               (21, @memberId, @organizationId, TRUE),
                               (22, @memberId, @organizationId, TRUE),
                               (23, @memberId, @organizationId, TRUE),
                               (24, @memberId, @organizationId, TRUE),
                               (25, @memberId, @organizationId, TRUE),
                               (26, @memberId, @organizationId, TRUE),
                               (27, @memberId, @organizationId, TRUE),
                               (28, @memberId, @organizationId, TRUE),
                               (29, @memberId, @organizationId, TRUE),
                               (30, @memberId, @organizationId, TRUE),
                               (31, @memberId, @organizationId, TRUE),
                               (32, @memberId, @organizationId, TRUE),
                               (33, @memberId, @organizationId, TRUE),
                               (34, @memberId, @organizationId, FALSE),
                               (35, @memberId, @organizationId, TRUE)";
                               
        await connection.ExecuteAsync(query, parameters);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}