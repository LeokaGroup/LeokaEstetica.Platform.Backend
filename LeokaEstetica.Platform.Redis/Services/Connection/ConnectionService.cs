using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using LeokaEstetica.Platform.Redis.Extensions;
using LeokaEstetica.Platform.Redis.Models.Chat;
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
    
    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task AddConnectionIdCacheAsync(string connectionId, string token)
    {
        await _redisCache.SetStringAsync(token, ProtoBufExtensions.Serialize(connectionId),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    /// <summary>
    /// Метод получает ConnectionId подключения для SignalR.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>ConnectionId.</returns>
    public async Task<string> GetConnectionIdCacheAsync(string key)
    {
        var connectionId = await _redisCache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(connectionId))
        {
            var newConnectionId = ProtoBufExtensions.Deserialize<string>(connectionId);
            
            // Данные нашли, продлеваем время жизни ключа.
            await _redisCache.RefreshAsync(key);

            return newConnectionId;
        }

        // В кэше нет ключа, добавляем.
        await _redisCache.SetStringAsync(key, ProtoBufExtensions.Serialize(key),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });

        var addedConnectionId = await _redisCache.GetStringAsync(key);
        connectionId = ProtoBufExtensions.Deserialize<string>(addedConnectionId);

        return connectionId;
    }

    /// <summary>
    /// Метод сохраняет список ConnectionId подключений SignalR в кэш.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="dialogRedis">Данные SignalR для хранения в кэше.</param>
    public async Task AddDialogMembersConnectionIdsCacheAsync(long dialogId, List<DialogRedis> dialogRedis)
    {
        await _redisCache.SetStringAsync(string.Concat("Dialog_", dialogId.ToString()),
            ProtoBufExtensions.Serialize(dialogRedis),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });
    }

    /// <summary>
    /// Метод получает список ConnectionId подключений SignalR из кэша.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>Список ConnectionId.</returns>
    public async Task<List<DialogRedis>> GetDialogMembersConnectionIdsCacheAsync(string key)
    {
        var connectionId = await _redisCache.GetStringAsync(string.Concat("Dialog_", key));

        if (!string.IsNullOrEmpty(connectionId))
        {
            var newConnectionId = ProtoBufExtensions.Deserialize<List<DialogRedis>>(connectionId);

            return newConnectionId;
        }

        return null;
    }
}