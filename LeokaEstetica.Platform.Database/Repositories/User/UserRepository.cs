using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Entities.User;

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
    /// <returns>Данные пользователя.</returns>
    public async Task<UserEntity> SaveUserAsync(UserEntity user)
    {
        var result = await _pgContext.Users.AddAsync(user);
        await _pgContext.SaveChangesAsync();

        return result.Entity;
    }
}