using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория ролей управления проектами.
/// </summary>
internal sealed class ProjectManagmentRoleRepository : BaseRepository, IProjectManagmentRoleRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider"></param>
    public ProjectManagmentRoleRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagementRoleOutput>?> GetUserRolesAsync(long? userId,
        long? projectId = null)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        IEnumerable<ProjectManagementRoleOutput>? result;

        // Получаем роли всех пользователей проекта компании.
        if (!userId.HasValue && projectId.HasValue)
        {
            parameters.Add("@projectId", projectId);

            var queryWithParameterProjectId = "SELECT pmr.role_id, " +
                                              "pmr.organization_id," +
                                              "pmr.project_member_id," +
                                              "pr.role_name," +
                                              "pr.role_sys_name," +
                                              "pmr.is_enabled," +
                                              "op.project_id," +
                                              "(SELECT \"Email\" " +
                                              "FROM dbo.\"Users\" " +
                                              "WHERE \"UserId\" = project_member_id) AS Email " +
                                              "FROM roles.project_roles AS pr " +
                                              "INNER JOIN roles.project_member_roles AS pmr " +
                                              "ON pr.role_id = pmr.role_id " +
                                              "INNER JOIN project_management.organizations AS po " +
                                              "ON pmr.organization_id = po.organization_id " +
                                              "INNER JOIN project_management.organization_projects AS op " +
                                              "ON po.organization_id = op.organization_id " +
                                              "WHERE op.project_id = @projectId " +
                                              "GROUP BY pmr.organization_id, pmr.project_member_id, pr.role_name," +
                                              " pr.role_sys_name, op.project_id, pmr.role_id, pmr.is_enabled " +
                                              "ORDER BY pmr.role_id ";
                                              
            result = await connection.QueryAsync<ProjectManagementRoleOutput>(queryWithParameterProjectId, parameters);
        }

        // Получаем роли пользователя.
        else
        {
            parameters.Add("@userId", userId);

            var queryWithParameterUser = "SELECT pmr.role_id, " +
                                         "pmr.organization_id," +
                                         "pmr.project_member_id," +
                                         "pr.role_name," +
                                         "pr.role_sys_name," +
                                         "pmr.is_enabled," +
                                         "op.project_id," +
                                         "(SELECT \"Email\" " +
                                         "FROM dbo.\"Users\" " +
                                         "WHERE \"UserId\" = project_member_id) AS Email " +
                                         "FROM roles.project_roles AS pr " +
                                         "INNER JOIN roles.project_member_roles AS pmr " +
                                         "ON pr.role_id = pmr.role_id " +
                                         "INNER JOIN project_management.organizations AS po " +
                                         "ON pmr.organization_id = po.organization_id " +
                                         "INNER JOIN project_management.organization_projects AS op " +
                                         "ON po.organization_id = op.organization_id " +
                                         "INNER JOIN project_management.organization_members AS om " +
                                         "ON op.organization_id = om.organization_id " +
                                         "WHERE om.member_id = @userId " +
                                         "GROUP BY pmr.organization_id, pmr.project_member_id, pr.role_name," +
                                         " pr.role_sys_name, op.project_id, pmr.role_id, pmr.is_enabled " +
                                         "ORDER BY pmr.role_id ";

            if (projectId.HasValue)
            {
                parameters.Add("@projectId", projectId);
                queryWithParameterUser += "AND op.project_id = @projectId";
            }

            result = await connection.QueryAsync<ProjectManagementRoleOutput>(queryWithParameterUser, parameters);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task UpdateRolesAsync(IEnumerable<ProjectManagementRoleInput> roles)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new List<DynamicParameters>();

        foreach (var r in roles)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@roleId", r.RoleId);
            tempParameters.Add("@enabled", r.IsEnabled);
            tempParameters.Add("@userId", r.UserId);

            parameters.Add(tempParameters);
        }

        var query = "UPDATE roles.project_member_roles " +
                    "SET is_enabled = @enabled " +
                    "WHERE role_id = @roleId " +
                    "AND project_member_id = @userId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<bool> GetProjectRoleByRoleSysNameAsync(string? roleSysName, long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@roleSysName", roleSysName);
        parameters.Add("@userId", userId);

        var query = "SELECT pmr.is_enabled " +
                    "FROM roles.project_member_roles AS pmr " +
                    "INNER JOIN roles.project_roles AS pr " +
                    "ON pmr.role_id = pr.role_id " +
                    "WHERE pmr.project_member_id = @userId " +
                    "AND pr.role_sys_name = @roleSysName";

        var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parameters);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}