using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.User;

/// <summary>
/// Класс реализует методы репозитория пользователей.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogService _logger;
    
    public UserRepository(PgContext pgContext, 
        ILogService logger)
    {
        _pgContext = pgContext;
        _logger = logger;
    }

    /// <summary>
    /// Метод сохраняет нового пользователя в базу.
    /// </summary>
    /// <param name="user">Данные пользователя для добавления.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> SaveUserAsync(UserEntity user)
    {
        await _pgContext.Users.AddAsync(user);
        await _pgContext.SaveChangesAsync();

        return user.UserId;
    }

    /// <summary>
    /// Метод находит пользователя по его UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserEntity> GetUserByUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод проверет существование пользователя по email в базе.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    public async Task<bool> CheckUserByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .AnyAsync(u => u.Email.Equals(email));

        return result;
    }

    /// <summary>
    /// Метод запишет код подтверждения пользователю.
    /// </summary>
    /// <param name="code">Код подтверждения, который мы отправили пользователю на почту.</param>
    /// <param name="userId">UserId.</param>
    public async Task SetConfirmAccountCodeAsync(Guid code, long userId)
    {
        var user = await GetUserByUserIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundUserByIdException(userId);
        }
        
        user.ConfirmEmailCode = code;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    public async Task<bool> ConfirmAccountAsync(Guid code)
    {
        var user = await _pgContext.Users.FirstOrDefaultAsync(u => u.ConfirmEmailCode.Equals(code));

        if (user is null)
        {
            throw new InvalidOperationException($"Не удалось подтвердить почту пользователя по коду {code}!");
        }

        user.EmailConfirmed = true;
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод получает хэш пароля для проверки пользователя.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<string> GetPasswordHashByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.PasswordHash)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его почте.
    /// </summary>
    /// <param name="account">Почта пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserByEmailAsync(string account)
    {
        try
        {
            var result = await _pgContext.Users
                .Where(u => u.Email.Equals(account))
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }
    
    /// <summary>
    /// Метод получает код пользователя по его почте.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<Guid> GetUserCodeByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.UserCode)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает основную информацию профиля по UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Основные данные профиля.</returns>
    public async Task<UserInfoOutput> GetUserPhoneEmailByUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => new UserInfoOutput
            {
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод сохраняет телефон и почту пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="phone">Номер телефона.</param>
    public async Task SaveUserPhoneAsync(long userId, string phone)
    {
        var user = await GetUserByUserIdAsync(userId);
        user.PhoneNumber = phone;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод находит логин и почту пользователей по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Список пользователей.</returns>
    public async Task<List<UserEntity>> GetUserByEmailOrLoginAsync(string searchText)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Contains(searchText) 
                        || u.Login.Contains(searchText))
            .Select(u => new UserEntity
            {
                Email = u.Email,
                Login = u.Login,
                UserId = u.UserId
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByEmailOrLoginAsync(string searchText)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Contains(searchText) 
                        || u.Login.Contains(searchText))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает словарь кодов пользователей.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    public async Task<Dictionary<long, Guid>> GetUsersCodesAsync()
    {
        var result = await _pgContext.Users.ToDictionaryAsync(k => k.UserId, v => v.UserCode);

        return result;
    }
}