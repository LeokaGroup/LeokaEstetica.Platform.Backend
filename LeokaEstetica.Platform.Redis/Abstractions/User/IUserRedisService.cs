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

    /// <summary>
    /// Метод добавляет в кэш данные для восстановления пароля пользователя.
    /// </summary>
    /// <param name="code">Код для подтверждения.</param>
    /// <param name="userId">Id пользователя.</param>
    Task AddRestoreUserDataCacheAsync(string code, long userId);

    /// <summary>
    /// Метод получает код восстановления пароля.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак успешной проверки.</returns>
    Task<bool> GetRestoreUserDataCacheAsync(long userId);

    /// <summary>
    /// Метод удаляет из кэша пользователей, которых ранее помечали к удалению.
    /// К этому моменту они уже удалены из БД, поэтому из кэша надо удалить тоже.
    /// </summary>
    Task DeleteMarkDeactivateUserAccountsAsync();
}