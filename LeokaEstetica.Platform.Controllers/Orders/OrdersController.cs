using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Services.Abstractions.Orders;
using LeokaEstetica.Platform.Services.Builders;
using Microsoft.AspNetCore.Mvc;

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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="ordersService">Сервис заказов пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    public OrdersController(IOrdersService ordersService, 
        IMapper mapper)
    {
        _ordersService = ordersService;
        _mapper = mapper;
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
}