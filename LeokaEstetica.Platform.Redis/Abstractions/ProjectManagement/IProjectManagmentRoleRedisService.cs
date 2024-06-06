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
    /// <returns>Список ролей.</returns>
    Task<IEnumerable<ProjectManagementRoleRedis>?> GetUserRolesAsync(long userId);
    
    /// <summary>
    /// Метод записывает в кэш список ролей пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="roles">Роли пользователя.</param>
    Task SetUserRolesAsync(long userId, IEnumerable<ProjectManagementRoleRedis> roles);
}