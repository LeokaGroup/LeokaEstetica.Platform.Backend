using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using LeokaEstetica.Platform.Redis.Enums;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Redis.Models.Chat;
using LeokaEstetica.Platform.Redis.Models.Common.Connection;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Redis.Services.Connection;

/// <summary>
/// Класс реализует методы сервиса подключений Redis.
/// </summary>
internal sealed class ConnectionService : IConnectionService
{
    private readonly IDistributedCache _redisCache;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redisCache">Кэш редиса.</param>
    public ConnectionService(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }
    
    /// <inheritdoc/>
    public async Task AddConnectionIdCacheAsync(string userCode, string connectionId, UserConnectionModuleEnum module)
    {
        await _redisCache.SetStringAsync(string.Concat(userCode + "_", module.ToString()),
            ProtoBufExtensions.Serialize(new UserConnection
            {
                ConnectionId = connectionId,
                Module = module
            }),
            new DistributedCacheEntryOptions
            {
                // При разлогине мы удаляем из кэша запись о пользователе, иначе живет 1 раб.день.
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
            });
    }

    /// <inheritdoc/>
    public async Task<UserConnection?> GetConnectionIdCacheAsync(string key)
    {
        var connection = await _redisCache.GetStringAsync(key);

        if (connection is not null)
        {
            var result = ProtoBufExtensions.Deserialize<UserConnection>(connection);

            if (result is null)
            {
                throw new InvalidOperationException("Ошибка каста к результату из кэша. " +
                                                    $"UserCode: {key}.");
            }
            
            // Данные нашли, продлеваем время жизни ключа.
            await _redisCache.RefreshAsync(key);
            
            result.IsCacheExists = true;
            
            return result;
        }
        
        return new UserConnection
        {
            IsCacheExists = false
        };
    }

    /// <inheritdoc/>
    public async Task AddDialogMembersConnectionIdsCacheAsync(long dialogId, List<DialogRedis> dialogRedis)
    {
        await _redisCache.SetStringAsync(string.Concat("Dialog_", dialogId.ToString()),
            ProtoBufExtensions.Serialize(dialogRedis),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });
    }

    /// <inheritdoc/>
    public async Task<List<DialogRedis>?> GetDialogMembersConnectionIdsCacheAsync(string key)
    {
        var connectionId = await _redisCache.GetStringAsync(string.Concat("Dialog_", key));

        if (!string.IsNullOrEmpty(connectionId))
        {
            var newConnectionId = ProtoBufExtensions.Deserialize<List<DialogRedis>>(connectionId);

            return newConnectionId;
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<UserConnection> CheckConnectionIdCacheAsync(Guid userCode, UserConnectionModuleEnum module)
    {
        var connection = await GetConnectionIdCacheAsync(string.Concat(userCode + "_", module.ToString()));

        if (connection is not null && !string.IsNullOrEmpty(connection.ConnectionId))
        {
            connection.Module = module;
            connection.IsCacheExists = true;
        }

        else
        {
            connection = new UserConnection
            {
                Module = module,
                IsCacheExists = false
            };
        }

        return connection;
    }
}