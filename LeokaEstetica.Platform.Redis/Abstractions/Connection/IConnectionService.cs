using LeokaEstetica.Platform.Redis.Enums;
using LeokaEstetica.Platform.Redis.Models.Chat;
using LeokaEstetica.Platform.Redis.Models.Common.Connection;

namespace LeokaEstetica.Platform.Redis.Abstractions.Connection;

/// <summary>
/// Абстракция подключений Redis.
/// </summary>
public interface IConnectionService
{
    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="module">Модуль приложения.</param>
    Task AddConnectionIdCacheAsync(string userCode, string connectionId, UserConnectionModuleEnum module);
    
    /// <summary>
    /// Метод получает ConnectionId подключения для SignalR.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>Выходная модель.</returns>
    Task<UserConnection?> GetConnectionIdCacheAsync(string key);

    /// <summary>
    /// Метод сохраняет список ConnectionId подключений SignalR в кэш.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="dialogRedis">Данные SignalR для хранения в кэше.</param>
    Task AddDialogMembersConnectionIdsCacheAsync(long dialogId, List<DialogRedis> dialogRedis);
    
    /// <summary>
    /// Метод получает список ConnectionId подключений SignalR из кэша.
    /// </summary>
    /// <param name="key">Ключ поиска.</param>
    /// <returns>Список ConnectionId.</returns>
    Task<List<DialogRedis>?> GetDialogMembersConnectionIdsCacheAsync(string key);
    
    /// <summary>
    /// Метод проверяет, есть ли в Redis такой код пользователя.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="module">Модуль приложения.</param>
    /// <returns>Выходная модель.</returns>
    Task<UserConnection> CheckConnectionIdCacheAsync(Guid userCode, UserConnectionModuleEnum module);
}