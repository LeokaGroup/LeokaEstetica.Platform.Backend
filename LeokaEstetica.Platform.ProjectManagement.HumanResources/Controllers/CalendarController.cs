using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Controllers;

/// <summary>
/// Контроллер работы с календарями.
/// </summary>
[ApiController]
[Route("project-management-human-resources/calendar")]
[AuthFilter]
public class CalendarController : BaseController
{
  private readonly ICalendarService _calendarService;

  /// <summary>
  /// Конструктор.
  /// </summary>
  /// <param name="calendarService">Сервис календарей.</param>
  public CalendarController(ICalendarService calendarService)
  {
    _calendarService = calendarService;
  }

  /// <summary>
  /// Метод получает события календаря текущего пользователя.
  /// </summary>
  /// <returns>Список событий.</returns>
  // [HttpGet]
  // [Route("events")]
  // [ProducesResponseType(200, Type = typeof(IEnumerable<CalendarOutput>))]
  // [ProducesResponseType(400)]
  // [ProducesResponseType(403)]
  // [ProducesResponseType(500)]
  // [ProducesResponseType(404)]
  // public async Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync()
  // {
  //   
  // }
}