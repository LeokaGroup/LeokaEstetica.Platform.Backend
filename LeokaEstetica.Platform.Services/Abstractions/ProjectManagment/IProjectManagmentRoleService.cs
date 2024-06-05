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
    /// <param name="projectId">Id проекта, если передали.</param>
    /// <returns>Список ролей.</returns>
    Task<IEnumerable<ProjectManagementRoleOutput>> GetUserRolesAsync(long? projectId = null);
}