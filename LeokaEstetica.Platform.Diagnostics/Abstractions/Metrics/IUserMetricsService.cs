using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;

/// <summary>
/// Абстракция метрик пользователей.
/// </summary>
public interface IUserMetricsService
{
    /// <summary>
    /// Метод получает список новых пользователей за текущий месяц.
    /// </summary>
    /// <returns>Список новых пользователей.</returns>
    Task<IEnumerable<UserEntity>> GetNewUsersAsync();
}