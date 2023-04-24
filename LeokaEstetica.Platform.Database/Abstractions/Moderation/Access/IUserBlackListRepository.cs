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
    /// Метод удаляет пользователя из ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    Task RemoveUserBlackListAsync(long userId);

    /// <summary>
    /// Метод получает список пользователей в ЧС.
    /// </summary>
    /// <returns>Список пользователей в ЧС.</returns>
    Task<(IEnumerable<UserEmailBlackListEntity>, IEnumerable<UserPhoneBlackListEntity>)> GetUsersBlackListAsync();
}