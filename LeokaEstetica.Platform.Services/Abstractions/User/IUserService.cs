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
}