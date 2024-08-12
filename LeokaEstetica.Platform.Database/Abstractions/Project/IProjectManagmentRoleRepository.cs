using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

namespace LeokaEstetica.Platform.Database.Abstractions.Project;

/// <summary>
/// Абстракция репозитория ролей управления проектами.
/// </summary>
public interface IProjectManagmentRoleRepository
{
    /// <summary>
    /// Метод получает список ролей пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="companyId">Id компании.</param>
    /// <returns>Список ролей.</returns>
    Task<IEnumerable<ProjectManagementRoleOutput>?> GetUserRolesAsync(long? userId, long? projectId, long? companyId);
    
    /// <summary>
    /// Метод обновляет роли пользователей.
    /// </summary>
    /// <param name="roles">Список ролей к обновлению.</param>
    Task UpdateRolesAsync(IEnumerable<ProjectManagementRoleInput> roles);

    /// <summary>
    /// Метод получает роль пользователя по ее системному названию.
    /// </summary>
    /// <param name="roleSysName">Системное название роли.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак нааличия роли.</returns>
    Task<bool> CheckProjectRoleAsync(string? roleSysName, long userId, long projectId);
}