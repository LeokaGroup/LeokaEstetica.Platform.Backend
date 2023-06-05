using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Metrics;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Metrics;

/// <summary>
/// Класс реализует методы репозитория метрик новых пользователей.
/// </summary>
public class UserMetricsRepository : IUserMetricsRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Консструктор.
    /// </summary>
    /// <param name="pgContext"></param>
    public UserMetricsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список новых пользователей за текущий месяц.
    /// </summary>
    /// <returns>Список новых пользователей.</returns>
    public async Task<IEnumerable<UserEntity>> GetNewUsersAsync()
    {
        var result = await _pgContext.Users
            .Where(u => u.DateRegister.Month == DateTime.Now.Month)
            .Select(u => new UserEntity
            {
                Email = u.Email,
                Login = u.Login
            })
            .ToListAsync();

        return result;
    }
}