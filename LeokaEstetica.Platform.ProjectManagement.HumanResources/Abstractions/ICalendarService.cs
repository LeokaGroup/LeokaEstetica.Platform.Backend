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
}