using LeokaEstetica.Platform.Models.Dto.Common.Cache;

namespace LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;

/// <summary>
/// Абстракция сервиса ролей управления проектами.
/// </summary>
public interface IProjectManagmentRoleRedisService
{
    /// <summary>
    /// Метод получает из кэша список ролей пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="companyId">Id компании.</param>
    /// <returns>Список ролей.</returns>
    Task<IEnumerable<ProjectManagementRoleRedis>?> GetUserRolesAsync(long userId, long projectId, long companyId);
    
    /// <summary>
    /// Метод записывает в кэш список ролей пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="companyId">Id компании.</param>
    /// <param name="roles">Роли пользователя.</param>
    Task SetUserRolesAsync(long userId, long projectId, long companyId, IEnumerable<ProjectManagementRoleRedis> roles);
}