using AutoMapper;
using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
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

    /// <summary>
    /// Метод получает список тикетов для профиля пользователя.
    /// </summary>
    /// <returns>Список тикетов.</returns>
    [HttpGet]
    [Route("profile")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TicketOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TicketOutput>> GetUserProfileTicketsAsync()
    {
        var result = await _ticketService.GetUserProfileTicketsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает список тикетов для КЦ.
    /// </summary>
    /// <returns>Список тикетов.</returns>
    [HttpGet]
    [Route("callcenter")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TicketOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TicketOutput>> GetCallCenterTicketsAsync()
    {
        var result = await _ticketService.GetCallCenterTicketsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает данные тикета.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <returns>Данные тикета.</returns>
    [HttpGet]
    [Route("ticket")]
    [ProducesResponseType(200, Type = typeof(SelectedTicketOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<SelectedTicketOutput> GetSelectedTicketAsync([FromQuery] long ticketId)
    {
        var validator = await new GetTicketValidator().ValidateAsync(ticketId);

        if (validator.Errors.Any())
        {
            var ex = new InvalidOperationException("Ошибка получения тикета.");
            _logger.LogError(ex, validator.Errors.First().ErrorMessage);

            throw ex;
        }

        var result = await _ticketService.GetSelectedTicketAsync(ticketId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод создает сообщение тикета.
    /// </summary>
    /// <param name="createMessageInput">Входная модель.</param>
    /// <returns>Список сообщений.</returns>
    [HttpPost]
    [Route("message")]
    [ProducesResponseType(200, Type = typeof(CreateTicketMessageOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateTicketMessageOutput> CreateTicketMessageAsync(
        [FromBody] CreateMessageInput createMessageInput)
    {
        var validator = await new CreateTicketMessageValidator().ValidateAsync(createMessageInput);
        var result = new CreateTicketMessageOutput
        {
            Messages = new List<TicketMessageOutput>()
        };

        // Если есть ошибки, то не даем создать сообщение.
        if (validator.Errors.Any())
        {
            foreach (var err in validator.Errors)
            {
                var ex = new InvalidOperationException("Ошибка создания сообщения тикета.");
                _logger.LogError(ex, err.ErrorMessage);
            }

            return result;
        }
        
        result = await _ticketService.CreateTicketMessageAsync(createMessageInput.TicketId,
            createMessageInput.Message, GetUserName());
        
        result.IsSuccess = true;

        return result;
    }

    /// <summary>
    /// Метод закрывает тикет (идет проставление статуса тикета "Закрыт").
    /// </summary>
    /// <param name="closeTicketInput">Входная модель.</param>
    [HttpPatch]
    [Route("close")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CloseTicketAsync([FromBody] CloseTicketInput closeTicketInput)
    {
        var ticketId = closeTicketInput.TicketId;
        var validator = await new CloseTicketValidator().ValidateAsync(ticketId);

        // Если есть ошибки, то не даем создать сообщение.
        if (validator.Errors.Any())
        {
            var ex = new InvalidOperationException("Ошибка создания сообщения тикета.");
            _logger.LogError(ex, validator.Errors.First().ErrorMessage);

            return;
        }

        await _ticketService.CloseTicketAsync(ticketId, GetUserName(), GetTokenFromHeader());
    }

    /// <summary>
    /// Метод создает предложение/пожелание.
    /// </summary>
    /// <param name="wisheOfferInput">Входная модель.</param>
    [AllowAnonymous]
    [HttpPost]
    [Route("wishes-offers")]
    public async Task<WisheOfferOutput> CreateWisheOfferAsync([FromBody] WisheOfferInput wisheOfferInput)
    {
        var result = new WisheOfferOutput { Errors = new List<ValidationFailure>() };
        
        var validator = await new CreateWisheOfferValidator().ValidateAsync(wisheOfferInput);

        // Если есть ошибки, то не даем создать сообщение.
        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();
            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException(exceptions);
            _logger.LogError(ex, "Ошибки создания пожелания/предложения.");
            
            result.Errors.AddRange(validator.Errors);

            return result;
        }
        
        result = await _ticketService.CreateWisheOfferAsync(wisheOfferInput.ContactEmail,
            wisheOfferInput.WisheOfferText);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}