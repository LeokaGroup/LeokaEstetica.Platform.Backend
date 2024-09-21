using LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Services;

/// <summary>
/// Класс реализует методы сервиса календарей.
/// </summary>
internal sealed class CalendarService : ICalendarService
{
    private readonly ILogger<CalendarService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    public CalendarService(ILogger<CalendarService> logger)
    {
        _logger = logger;
    }
}