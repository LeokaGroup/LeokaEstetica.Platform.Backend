using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.User;

/// <summary>
/// Класс реализует методы репозитория пользователей.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly PgContext _pgContext;
    
    public UserRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
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
}