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
    /// <param name="projectId">Id проекта, если передали.</param>
    /// <returns>Список ролей.</returns>
    Task<IEnumerable<ProjectManagementRoleOutput>?> GetUserRolesAsync(long? userId, long? projectId = null);
    
    /// <summary>
    /// Метод обновляет роли пользователей.
    /// </summary>
    /// <param name="roles">Список ролей к обновлению.</param>
    Task UpdateRolesAsync(IEnumerable<ProjectManagementRoleInput> roles);
}