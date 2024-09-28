using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Validators;
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
    private readonly ILogger<CalendarController> _logger;
    private readonly Lazy<IDiscordService> _discordService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="calendarService">Сервис календарей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    public CalendarController(ICalendarService calendarService,
        ILogger<CalendarController> logger,
        Lazy<IDiscordService> discordService)
    {
        _calendarService = calendarService;
        _logger = logger;
        _discordService = discordService;
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
    [ProducesResponseType(200, Type = typeof(List<BusyVariantOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public Task<List<BusyVariantOutput>> GetBusyVariantsAsync()
    {
        return Task.FromResult(new List<BusyVariantOutput>
        {
            new()
            {
                Description = CalendarEventMemberStatusEnum.Busy.GetEnumDescription(),
                SysName = CalendarEventMemberStatusEnum.Busy.ToString()
            },
            new()
            {
                Description = CalendarEventMemberStatusEnum.MayBeBusy.GetEnumDescription(),
                SysName = CalendarEventMemberStatusEnum.MayBeBusy.ToString()
            },
            new()
            {
                Description = CalendarEventMemberStatusEnum.Available.GetEnumDescription(),
                SysName = CalendarEventMemberStatusEnum.Available.ToString()
            },
        });
    }

    /// <summary>
    /// Метод создает событие календаря.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    [HttpPost]
    [Route("event")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateCalendarEventAsync([FromBody] CalendarInput calendarInput)
    {
        var validator = await new CreateEventValidator().ValidateAsync(calendarInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();
            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException(exceptions);
            _logger.LogError(ex, "Ошибки при создании события календаря.");

            throw ex;
        }

        await _calendarService.CreateCalendarEventAsync(calendarInput, GetUserName());
    }

    /// <summary>
    /// Метод получает детали события календаря.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <returns>Детали события календаря.</returns>
    [HttpGet]
    [Route("event")]
    [ProducesResponseType(200, Type = typeof(CalendarOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CalendarOutput> GetEventDetailsAsync([FromQuery] long eventId)
    {
        var result = await _calendarService.GetEventDetailsAsync(eventId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод обновляет событие календаря.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    [HttpPut]
    [Route("event")]
    [ProducesResponseType(200, Type = typeof(CalendarOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateEventAsync([FromBody] CalendarInput calendarInput)
    {
        var validator = await new UpdateEventValidator().ValidateAsync(calendarInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();
            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException(exceptions);
            _logger.LogError(ex, "Ошибки при обновлении события календаря.");

            throw ex;
        }

        if (!calendarInput.EventId.HasValue)
        {
            var ex = new InvalidOperationException("Id события не передан.");
            _logger.LogError(ex, "Ошибки при обновлении события календаря.");

            await _discordService.Value.SendNotificationErrorAsync(ex);
        }

        await _calendarService.UpdateEventAsync(calendarInput, GetUserName());
    }
}