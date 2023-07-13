using System.Security.Claims;
using LeokaEstetica.Platform.Models.Dto.Output.User;

namespace LeokaEstetica.Platform.Access.Abstractions.User;

/// <summary>
/// Абстракция сервиса проверки доступа пользователей.
/// </summary>
public interface IAccessUserService
{
    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <param name="isVkAuth">Признак блокировки через ВК.</param>
    /// <returns>Признак блокировки.</returns>
    Task<bool> CheckBlockedUserAsync(string availableBlockedText, bool isVkAuth);

    /// <summary>
    /// Метод проверяет, заполнена ли анкета пользователя.
    /// Если не заполнена, то запрещаем доступ к ключевому функционалу.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <returns>Признак проверки.</returns>
    Task<bool> IsProfileEmptyAsync(long userId);
    
    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <returns>Токен пользователя.</returns>
    Task<ClaimsIdentity> GetIdentityClaimAsync(string email);

    /// <summary>
    /// Метод создает токен пользователю.
    /// </summary>
    /// <param name="claimsIdentity">Объект полномочий.</param>
    /// <returns>Строка токена.</returns>
    Task<string> CreateTokenFactoryAsync(ClaimsIdentity claimsIdentity);

    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Токен пользователя.</returns>
    Task<ClaimsIdentity> GetIdentityClaimVkUserAsync(long vkUserId);
    
    /// <summary>
    /// Метод обновляет токен.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Новые данные авторизации.</returns>
    Task<UserSignInOutput> RefreshTokenAsync(string account);
}