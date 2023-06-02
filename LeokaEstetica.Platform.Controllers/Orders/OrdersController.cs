using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Orders;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Services.Abstractions.Orders;
using LeokaEstetica.Platform.Services.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Controllers.Orders;

/// <summary>
/// Контроллер заказов пользователя.
/// </summary>
[AuthFilter]
[ApiController]
[Route("orders")]
public class OrdersController : BaseController
{
    private readonly IOrdersService _ordersService;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdersController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="ordersService">Сервис заказов пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="logger">Логгер.</param>
    public OrdersController(IOrdersService ordersService, 
        IMapper mapper, 
        ILogger<OrdersController> logger)
    {
        _ordersService = ordersService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Метод получает список заказов пользователя.
    /// </summary>
    /// <returns>Список заказов пользователя.</returns>
    [HttpGet]
    [Route("all")]
    [ProducesResponseType(200, Type = typeof(GetOrderResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<GetOrderResult> GetUserOrdersAsync()
    {
        var result = new GetOrderResult { Orders = new List<OrderOutput>() };
        var items = await _ordersService.GetUserOrdersAsync(GetUserName());

        if (items is null)
        {
            return result;
        }
        
        var orders = items.ToList();
        
        if (!orders.Any())
        {
            return result;
        }

        result.Orders = CreateUserOrdersBuilder.Create(orders, _mapper);

        return result;
    }

    /// <summary>
    /// Метод получает детали заказа по его Id.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns>Детали заказа.</returns>
    [HttpGet]
    [Route("details")]
    [ProducesResponseType(200, Type = typeof(OrderOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<OrderOutput> GetOrderDetailsAsync([FromQuery] long orderId)
    {
        var validator = await new GetOrderDetailsValidator().ValidateAsync(orderId);

        if (validator.Errors.Any())
        {
            var errorMessage = validator.Errors.First().ErrorMessage;
            _logger.LogError(errorMessage);
            
            throw new InvalidOperationException(errorMessage);
        }

        var order = await _ordersService.GetOrderDetailsAsync(orderId, GetUserName());
        var result = _mapper.Map<OrderOutput>(order);

        return result;
    }
}