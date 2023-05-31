using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера заказов пользователя.
/// </summary>
public static class CreateUserOrdersBuilder
{
    /// <summary>
    /// Список заказов пользователя.
    /// </summary>
    private static readonly List<OrderOutput> _orders = new();

    /// <summary>
    /// Метод форматирует даты заказов к нужному виду.
    /// </summary>
    /// <param name="orders">Список заказов из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<OrderOutput> Create(IEnumerable<OrderEntity> orders, IMapper mapper)
    {
        _orders.Clear();
        
        foreach (var item in orders)
        {
            // Прежде чем мапить форматируем даты.
            var convertDateCreate = item.DateCreated.ToString("g", CultureInfo.GetCultureInfo("ru"));

            // Затем уже мапим к результирующей модели.
            var newItem = mapper.Map<OrderOutput>(item);
            newItem.OrderId = newItem.OrderId;
            newItem.DateCreated = convertDateCreate;
            newItem.OrderName = item.OrderName;
            newItem.OrderDetails = item.OrderDetails;
            newItem.Price = item.Price;
            newItem.StatusName = item.StatusName;
            
            _orders.Add(newItem);
        }

        return _orders;
    }
}