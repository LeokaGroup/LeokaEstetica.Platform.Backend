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
    Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync(long userId);
}