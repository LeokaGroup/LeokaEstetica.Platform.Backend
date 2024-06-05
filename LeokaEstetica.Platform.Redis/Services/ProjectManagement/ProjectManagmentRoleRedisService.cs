using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Redis.Models.ProjectManagement;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.ProjectManagement;

/// <summary>
/// Класс реализует методы сервиса ролей управления проектами.
/// </summary>
internal sealed class ProjectManagmentRoleRedisService : IProjectManagmentRoleRedisService
{
    private readonly IDistributedCache _redis;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redis">Кэш.</param>
    public ProjectManagmentRoleRedisService(IDistributedCache redis)
    {
        _redis = redis;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagementRoleRedis>> GetUserRolesAsync(long userId)
    {
        var key = string.Concat(userId, CacheConst.Cache.PROJECT_MANAGEMENT_USER_ROLES);
        var items = await _redis.GetStringAsync(key);

        if (string.IsNullOrEmpty(items))
        {
            return Enumerable.Empty<ProjectManagementRoleRedis>();
        }
        
        var result = ProtoBufExtensions.Deserialize<IEnumerable<ProjectManagementRoleRedis>>(items);

        return result;
    }

    /// <inheritdoc />
    public async Task SetUserRolesAsync(long userId, IEnumerable<ProjectManagementRoleRedis> roles)
    {
        var key = string.Concat(userId, CacheConst.Cache.PROJECT_MANAGEMENT_USER_ROLES);
        await _redis.SetStringAsync(key, ProtoBufExtensions.Serialize(roles), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        });
    }
}