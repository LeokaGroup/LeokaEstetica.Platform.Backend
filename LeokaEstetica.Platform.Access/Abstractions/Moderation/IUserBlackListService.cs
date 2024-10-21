using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;

namespace LeokaEstetica.Platform.Access.Abstractions.Moderation;

/// <summary>
/// Абстракция сервиса ЧС пользователей.
/// </summary>
public interface IUserBlackListService
{
    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    /// <param name="vkUserId">Id пользователя в системе ВКонтакте.</param>
    Task AddUserBlackListAsync(long userId, string? email, string? phoneNumber, long? vkUserId);

    /// <summary>
    /// Метод получает список пользователей в ЧС.
    /// </summary>
    /// <returns>Список пользователей в ЧС.</returns>
    Task<UserBlackListResult> GetUsersBlackListAsync();
}