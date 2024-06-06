using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Redis.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

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

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagementRoleRedis>?> GetUserRolesAsync(long userId)
    {
        var key = CacheConst.Cache.PROJECT_MANAGEMENT_USER_ROLES + userId + "_ProjectManagementRoles";
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
        var key = CacheConst.Cache.PROJECT_MANAGEMENT_USER_ROLES + userId + "_ProjectManagementRoles";
        await _redis.SetStringAsync(key,
            ProtoBufExtensions.Serialize(roles), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}