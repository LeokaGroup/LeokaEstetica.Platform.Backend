using LeokaEstetica.Platform.Models.Entities.Access;

namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;

/// <summary>
/// Абстракция репозитория ЧС пользователей.
/// </summary>
public interface IUserBlackListRepository
{
    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    Task AddUserBlackListAsync(long userId, string email, string phoneNumber);

    /// <summary>
    /// Метод получает список пользователей в ЧС.
    /// </summary>
    /// <returns>Список пользователей в ЧС.</returns>
    Task<(IEnumerable<UserEmailBlackListEntity>, IEnumerable<UserPhoneBlackListEntity>)> GetUsersBlackListAsync();

    /// <summary>
    /// Метод проверяет наличие пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наличие пользователя в чс.</returns>
    Task<bool> IsUserExistAsync(long userId);


    /// <summary>
    /// Метод проверяет, заблокирован ли пользователь по email и phone number
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Признак наличия пользователя в чс по email и phone number</returns>
    Task<bool> IsUserBlocked(long userId);
}