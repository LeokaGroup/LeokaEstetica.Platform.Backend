using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Database.Abstractions.Metrics;

/// <summary>
/// Абстракция репозитория метрик новых пользователей.
/// </summary>
public interface IUserMetricsRepository
{
    /// <summary>
    /// Метод получает список новых пользователей за текущий месяц.
    /// </summary>
    /// <returns>Список новых пользователей.</returns>
    Task<IEnumerable<UserEntity>> GetNewUsersAsync();
}