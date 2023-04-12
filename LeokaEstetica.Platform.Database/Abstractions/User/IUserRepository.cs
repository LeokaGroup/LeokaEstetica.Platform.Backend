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
    Task<long> AddUserAsync(UserEntity user); 

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
    /// Метод получает код пользователя по его VkUserId.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Хэш пароля.</returns>
    Task<Guid> GetUserCodeByVkUserIdAsync(long vkUserId);

    /// <summary>
    /// Метод получает недостающую информацию профиля по UserId.
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

    /// <summary>
    /// Метод получает словарь кодов пользователей.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    Task<Dictionary<long, Guid>> GetUsersCodesAsync();

    /// <summary>
    /// Метод получает список пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    Task<List<UserEntity>> GetAllAsync();

    /// <summary>
    /// Метод находит Id пользователя по его коду.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByCodeAsync(string userCode);
    
    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByEmailAsync(string email);
    
    /// <summary>
    /// Метод находит Id пользователя по его номеру телефона.
    /// </summary>
    /// <param name="phoneNumber">Номер телефона пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByPhoneNumberAsync(string phoneNumber);
    
    /// <summary>
    /// Метод находит Id пользователя по его логину.
    /// </summary>
    /// <param name="phoneNumber">Логин пользователя.</param>
    /// <returns>Id пользователя.</returns>
    Task<long> GetUserIdByLoginAsync(string login);

    /// <summary>
    /// Метод проверет существование пользователя по VkUserId в базе.
    /// </summary>
    /// <param name="userId">VkUserId пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    Task<bool> CheckUserByVkUserIdAsync(long userId);

    /// <summary>
    /// Метод проставляет пользователям метку к удалению аккаунтов.
    /// </summary>
    /// <param name="users">Список пользователей, которых предупредим.</param>
    Task SetMarkDeactivateAccountsAsync(List<UserEntity> users);
}