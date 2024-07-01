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

            var queryWithParameterProjectId = "SELECT role_id, " +
                                              "organization_id," +
                                              "organization_member_id," +
                                              "role_name," +
                                              "role_sys_name," +
                                              "is_active," +
                                              "is_enabled," +
                                              "project_id," +
                                              "(SELECT \"Email\" " +
                                              "FROM dbo.\"Users\" " +
                                              "WHERE \"UserId\" = organization_member_id) AS Email " +
                                              "FROM roles.organization_project_member_roles " +
                                              "WHERE project_id = @projectId " +
                                              "GROUP BY organization_id, organization_member_id, role_name," +
                                              " role_sys_name, project_id, role_id " +
                                              "ORDER BY role_id ";
                                              
            result = await connection.QueryAsync<ProjectManagementRoleOutput>(queryWithParameterProjectId, parameters);
        }

        // Получаем роли пользователя.
        else
        {
            parameters.Add("@userId", userId);

            var queryWithParameterUser = "SELECT role_id, " +
                                         "organization_id," +
                                         "organization_member_id," +
                                         "role_name," +
                                         "role_sys_name," +
                                         "is_active," +
                                         "is_enabled," +
                                         "project_id " +
                                         "FROM roles.organization_project_member_roles " +
                                         "WHERE organization_member_id = @userId ";

            if (projectId.HasValue)
            {
                parameters.Add("@projectId", projectId);
                queryWithParameterUser += "AND project_id = @projectId";
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

        var query = "UPDATE roles.organization_project_member_roles " +
                    "SET is_enabled = @enabled " +
                    "WHERE role_id = @roleId " +
                    "AND organization_member_id = @userId";

        await connection.ExecuteAsync(query, parameters);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}