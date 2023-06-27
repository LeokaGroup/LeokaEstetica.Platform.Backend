using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Ticket;
using LeokaEstetica.Platform.Models.Dto.Input.Ticket;
using LeokaEstetica.Platform.Models.Dto.Output.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Controllers.Ticket;

/// <summary>
/// Контроллер работы с тикетами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("tickets")]
public class TicketController : BaseController
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;
    private readonly ILogger<TicketController> _logger;

    /// <summary>
    /// Конструктор.
    /// <param name="ticketService">Сервис тикетов.</param>
    /// <param name="ticketService">Автомаппер.</param>
    /// <param name="logger">Логер.</param>
    /// </summary>
    public TicketController(ITicketService ticketService, 
        IMapper mapper, 
        ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список категорий тикетов.
    /// </summary>
    /// <returns>Категории тикетов.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("categories")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TicketCategoryOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TicketCategoryOutput>> GetTicketCategoriesAsync()
    {
        var items = await _ticketService.GetTicketCategoriesAsync();
        var result = _mapper.Map<IEnumerable<TicketCategoryOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод создает тикет.
    /// </summary>
    /// <param name="createTicketInput">Входная модель.</param>
    [AllowAnonymous]
    [HttpPost]
    [Route("ticket")]
    public async Task<bool> CreateTicketAsync([FromBody] CreateTicketInput createTicketInput)
    {
        var validator = await new CreateTicketValidator().ValidateAsync(createTicketInput);

        if (validator.Errors.Any())
        {
            foreach (var err in validator.Errors)
            {
                var ex = new InvalidOperationException("Ошибка создания тикета.");
                _logger.LogError(ex, err.ErrorMessage);
            }
            
            return false;
        }

        await _ticketService.CreateTicketAsync(createTicketInput.Title, createTicketInput.Message, GetUserName());

        return true;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}