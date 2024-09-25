using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Enums;
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
    [HttpGet]
    [Route("events")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CalendarOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync()
    {
        var result = await _calendarService.GetCalendarEventsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает типы занятости.
    /// </summary>
    /// <returns>Список типов занятости.</returns>
    [HttpGet]
    [Route("busy-variants")]
    [ProducesResponseType(200, Type = typeof(Dictionary<int, string>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public Task<List<(int Id, string Value)>> GetBusyVariantsAsync()
    {
        return Task.FromResult(new List<(int Id, string Value)>
        {
            new()
            {
                Id = (int)CalendarEventMemberStatusEnum.Busy,
                Value = CalendarEventMemberStatusEnum.Busy.GetEnumDescription()
            },
            new()
            {
                Id = (int)CalendarEventMemberStatusEnum.MayBeBusy,
                Value = CalendarEventMemberStatusEnum.MayBeBusy.GetEnumDescription()
            },
            new()
            {
                Id = (int)CalendarEventMemberStatusEnum.Available,
                Value = CalendarEventMemberStatusEnum.Available.GetEnumDescription()
            }
        });
    }
}