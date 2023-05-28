using LeokaEstetica.Platform.Models.Entities.User;

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

    /// <summary>
    /// Метод добавляет в кэш пользователей, аккаунты которых нужно удалить и все их данные.
    /// </summary>
    /// <param name="users">Список пользователей.</param>
    Task AddMarkDeactivateUserAccountsAsync(List<UserEntity> users);
    
    /// <summary>
    /// Метод получает из кэша пользователей, аккаунты которых нужно удалить и все их данные.
    /// </summary>
    /// <param name="key">Ключ для получения списка аккаунтов.</param>
    /// <returns>Список пользователей для удаления их аккаунтов.</returns>
    Task<List<UserEntity>> GetMarkDeactivateUserAccountsAsync(string key);
}