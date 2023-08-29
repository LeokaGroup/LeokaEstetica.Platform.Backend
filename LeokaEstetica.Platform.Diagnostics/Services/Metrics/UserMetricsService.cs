using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Database.Abstractions.Metrics;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Diagnostics.Services.Metrics;

/// <summary>
/// Класс реализует методы сервиса метрик пользователей.
/// </summary>
internal sealed class UserMetricsService : IUserMetricsService
{
    private readonly ILogger<UserMetricsService> _logger;
    private readonly IUserMetricsRepository _userMetricsRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userMetricsRepository">Репозиторий метрик новых пользователей.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public UserMetricsService(ILogger<UserMetricsService> logger, 
        IUserMetricsRepository userMetricsRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _userMetricsRepository = userMetricsRepository;
        _userRepository = userRepository;
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
            var users = result.ToList();

            var updatedUserLogins = new List<UserEntity>();
            
            // Заменяем почту на логин. Если у пользователя нет логина в БД, то берем из почты до симврола @
            // и запишем в БД пользователю логин, если в БД его еще нет.
            // Иначе берем логин.
            foreach (var u in users)
            {
                // Если нет логина, то берем из почты до символа @.
                if (string.IsNullOrEmpty(u.Login))
                {
                    var email = u.Email;
                    var userId = await _userRepository.GetUserIdByEmailAsync(email);
                    var user = await _userRepository.GetUserByUserIdAsync(userId);
                
                    // Заполняем данные пользователя.
                    user.Login = email.Substring(0, email.IndexOf('@'));
                    user.UserId = userId;
                    
                    updatedUserLogins.Add(user);   
                }
            }

            // Если есть, каким пользователям нужно обновить логин.
            if (updatedUserLogins.Any())
            {
                await _userRepository.UpdateUsersLoginAsync(updatedUserLogins);
            }

            return users;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}