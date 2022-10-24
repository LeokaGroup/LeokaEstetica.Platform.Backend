namespace LeokaEstetica.Platform.Redis.Abstractions;

/// <summary>
/// Абстракция работы с кэшем Redis.
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SaveConnectionIdCacheAsync(string connectionId, string userCode);
}