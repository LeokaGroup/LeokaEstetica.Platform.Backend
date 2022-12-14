using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Database.Abstractions.User;

/// <summary>
/// Абстракция репозитория пользователей.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Метод сохраняет нового пользователя в базу.
    /// </summary>
    /// <param name="user">Данные пользователя для добавления.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> SaveUserAsync(UserEntity user); 

    /// <summary>
    /// Метод находит пользователя по его UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    Task<UserEntity> GetUserByUserIdAsync(long userId);

    /// <summary>
    /// Метод проверет существование пользователя по email в базе.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    Task<bool> CheckUserByEmailAsync(string email);

    /// <summary>
    /// Метод запишет код подтверждения пользователю.
    /// </summary>
    /// <param name="code">Код подтверждения, который мы отправили пользователю на почту.</param>
    /// <param name="userId">UserId.</param>
    Task SetConfirmAccountCodeAsync(Guid code, long userId);
    
    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    Task<bool> ConfirmAccountAsync(Guid code);

    /// <summary>
    /// Метод получает хэш пароля для проверки пользователя.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    Task<string> GetPasswordHashByEmailAsync(string email);

    /// <summary>
    /// Метод находит Id пользователя по его почте.
    /// </summary>
    /// <param name="account">Почта пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserByEmailAsync(string account);

    /// <summary>
    /// Метод получает код пользователя по его почте.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    Task<Guid> GetUserCodeByEmailAsync(string email);

    /// <summary>
    /// Метод получает основную информацию профиля по UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Основные данные профиля.</returns>
    Task<UserInfoOutput> GetUserPhoneEmailByUserIdAsync(long userId);

    /// <summary>
    /// Метод сохраняет телефон пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="phone">Номер телефона.</param>
    Task SaveUserPhoneAsync(long userId, string phone);

    /// <summary>
    /// Метод находит логин и почту пользователей по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Список пользователей.</returns>
    Task<List<UserEntity>> GetUserByEmailOrLoginAsync(string searchText);
    
    /// <summary>
    /// Метод находит Id пользователя по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByEmailOrLoginAsync(string searchText);
}