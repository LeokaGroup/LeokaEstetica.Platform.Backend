using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagmentHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Services;

/// <summary>
/// Класс реализует методы сервиса календарей.
/// </summary>
internal sealed class CalendarService : ICalendarService
{
    private readonly ILogger<CalendarService>? _logger;
    private readonly IUserRepository _userRepository;
    private readonly ICalendarRepository _calendarRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="calendarRepository">Репозиторий календаря.</param>
    public CalendarService(ILogger<CalendarService>? logger,
        IUserRepository userRepository,
        ICalendarRepository calendarRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _calendarRepository = calendarRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var result = await _calendarRepository.GetCalendarEventsAsync(userId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}