using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagmentHumanResources;

/// <summary>
/// Абстракция репозитория календарей.
/// </summary>
public interface ICalendarRepository
{
    /// <summary>
    /// Метод получает события календаря текущего пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список событий.</returns>
    Task<IEnumerable<CalendarOutput>?> GetCalendarEventsAsync(long userId);

    /// <summary>
    /// Метод получает участников событий.
    /// </summary>
    /// <param name="eventIds">Id событий.</param>
    /// <returns>Список участников.</returns>
    Task<IEnumerable<EventMemberOutput>?> GetEventMembersAsync(IEnumerable<long> eventIds);

    /// <summary>
    /// Метод получает роли участников событий.
    /// </summary>
    /// <param name="eventIds">Id событий.</param>
    /// <param name="eventMemberIds">Id участников событий.</param>
    /// <returns>Роли участников событий.</returns>
    Task<IEnumerable<EventMemberRoleOutput>?> GetEventMemberRolesAsync(IEnumerable<long> eventIds,
        IEnumerable<long> eventMemberIds);

    /// <summary>
    /// Метод создает событие календаря.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    Task CreateCalendarEventAsync(CalendarInput calendarInput);
    
    /// <summary>
    /// Метод получает детали события календаря.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <returns>Детали события календаря.</returns>
    Task<CalendarOutput> GetEventDetailsAsync(long eventId);
    
    /// <summary>
    /// Метод обновляет событие календаря.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    Task UpdateEventAsync(CalendarInput calendarInput);
}