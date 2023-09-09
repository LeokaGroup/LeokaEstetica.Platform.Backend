using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using LeokaEstetica.Platform.Redis.Extensions;
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
}