using LeokaEstetica.Platform.Models.Dto.Output.User;

namespace LeokaEstetica.Platform.Services.Abstractions.User;

/// <summary>
/// Абстракция сервиса пользователей.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Метод создает нового пользователя.
    /// </summary>
    /// <param name="password">Пароль. Он не хранится в БД. Хранится только его хэш.</param>
    /// <param name="email">Почта пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    Task<UserSignUpOutput> CreateUserAsync(string password, string email);

    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    Task<bool> ConfirmAccountAsync(Guid code);

    /// <summary>
    /// Метод авторизует пользователя.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Данные авторизации.</returns>
    Task<UserSignInOutput> SignInAsync(string email, string password);

    /// <summary>
    /// Метод обновляет токен.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Новые данные авторизации.</returns>
    Task<UserSignInOutput> RefreshTokenAsync(string account);

    /// <summary>
    /// Метод авторизации через Google. Если аккаунт не зарегистрирован в системе,
    /// то создаем также аккаунт используя данные аккаунта Google пользователя.
    /// </summary>
    /// <param name="googleAuthToken">Токен с данными пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    Task<UserSignInOutput> SignInAsync(string googleAuthToken);

    /// <summary>
    /// Метод авторизации через VK. Если аккаунт не зарегистрирован в системе,
    /// то создаем также аккаунт используя данные аккаунта DR пользователя.
    /// </summary>
    /// <param name="vkUserId">Id пользователя в системе ВК.</param>
    /// <param name="firstName">Имя пользователя в системе ВК.</param>
    /// <param name="firstName">Фамилия пользователя в системе ВК.</param>
    /// <returns>Данные пользователя.</returns>
    Task<UserSignInOutput> SignInAsync(long vkUserId, string firstName, string lastName);
}