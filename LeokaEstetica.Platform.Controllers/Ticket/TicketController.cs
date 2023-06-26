using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    
    /// <summary>
    /// Конструктор.
    /// <param name="ticketService">Сервис тикетов.</param>
    /// <param name="ticketService">Автомаппер.</param>
    /// </summary>
    public TicketController(ITicketService ticketService, 
        IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
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

    #endregion

    #region Приватные методы.

    

    #endregion
}