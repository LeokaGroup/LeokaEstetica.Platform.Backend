using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса ролей управления проектами.
/// </summary>
public interface IProjectManagmentRoleService
{
    /// <summary>
    /// Метод получает список ролей пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="companyId">Id компании.</param>
    /// <returns>Список ролей.</returns>
    Task<IEnumerable<ProjectManagementRoleOutput>> GetUserRolesAsync(string account, long projectId, long companyId);

    /// <summary>
    /// Метод обновляет роли пользователей.
    /// </summary>
    /// <param name="roles">Список ролей к обновлению.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateRolesAsync(IEnumerable<ProjectManagementRoleInput> roles, string account);
}