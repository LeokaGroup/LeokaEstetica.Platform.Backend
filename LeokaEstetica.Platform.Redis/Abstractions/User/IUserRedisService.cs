namespace LeokaEstetica.Platform.Redis.Abstractions.User;

/// <summary>
/// Абстракция сервиса кэша пользователей.
/// </summary>
public interface IUserRedisService
{
    /// <summary>
    /// Метод добавляет в кэш Id и токен пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    Task AddUserTokenAndUserIdCacheAsync(long userId, string token);

    /// <summary>
    /// Метод получает Id пользователя из кэша.
    /// </summary>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Id пользователя из кэша.</returns>
    Task<string> GetUserIdCacheAsync(string token);
}