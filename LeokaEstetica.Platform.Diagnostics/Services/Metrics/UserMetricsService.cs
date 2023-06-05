using LeokaEstetica.Platform.Database.Abstractions.Metrics;
using LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Diagnostics.Services.Metrics;

/// <summary>
/// Класс реализует методы сервиса метрик пользователей.
/// </summary>
public class UserMetricsService : IUserMetricsService
{
    private readonly ILogger<UserMetricsService> _logger;
    private readonly IUserMetricsRepository _userMetricsRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userMetricsRepository">Репозиторий метрик новых пользователей.</param>
    public UserMetricsService(ILogger<UserMetricsService> logger, 
        IUserMetricsRepository userMetricsRepository)
    {
        _logger = logger;
        _userMetricsRepository = userMetricsRepository;
    }

    /// <summary>
    /// Метод получает список новых пользователей за текущий месяц.
    /// </summary>
    /// <returns>Список новых пользователей.</returns>
    public async Task<IEnumerable<UserEntity>> GetNewUsersAsync()
    {
        try
        {
            var result = await _userMetricsRepository.GetNewUsersAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}