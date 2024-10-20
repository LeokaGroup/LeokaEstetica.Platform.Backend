﻿using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;

/// <summary>
/// Абстракция сервиса календарей.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Метод получает события календаря текущего пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список событий.</returns>
    Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync(string account);

    /// <summary>
    /// Метод создает событие календаря.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    Task CreateCalendarEventAsync(CalendarInput calendarInput, string account);

    /// <summary>
    /// Метод получает детали события календаря.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Детали события календаря.</returns>
    Task<CalendarOutput> GetEventDetailsAsync(long eventId, string account);

    /// <summary>
    /// Метод обновляет событие календаря.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    Task UpdateEventAsync(CalendarInput calendarInput, string account);

    /// <summary>
    /// Метод удаляет событие календаря.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    Task RemoveEventAsync(long eventId, string account);
}