using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
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
    
}