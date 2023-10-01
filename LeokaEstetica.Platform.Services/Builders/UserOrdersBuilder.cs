using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера заказов пользователя.
/// </summary>
public static class UserOrdersBuilder
{
    /// <summary>
    /// Метод форматирует даты заказов к нужному виду.
    /// </summary>
    /// <param name="orders">Список заказов из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<OrderOutput> CreateUserOrdersResult(List<OrderEntity> orders, IMapper mapper)
    {
        var result = new List<OrderOutput>(orders.Count);
        
        foreach (var item in orders)
        {
            // Затем уже мапим к результирующей модели.
            var newItem = mapper.Map<OrderOutput>(item);
            newItem.OrderId = newItem.OrderId;
            newItem.DateCreated = item.DateCreated.ToString("g", CultureInfo.GetCultureInfo("ru"));
            newItem.OrderName = item.OrderName;
            newItem.OrderDetails = item.OrderDetails;
            newItem.Price = item.Price;
            newItem.StatusName = item.StatusName;
            
            result.Add(newItem);
        }

        return result;
    }

    /// <summary>
    /// Метод форматирует даты заказов к нужному виду.
    /// </summary>
    /// <param name="histories">Список заказов из БД.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<HistoryOutput> CreateHistoryResult(List<HistoryEntity> histories, IMapper mapper)
    {
        var result = new List<HistoryOutput>(histories.Count);

        foreach (var item in histories)
        {
            // Затем уже мапим к результирующей модели.
            var newItem = mapper.Map<HistoryOutput>(item);
            newItem.DateCreated = item.DateCreated.ToString("g", CultureInfo.GetCultureInfo("ru"));

            result.Add(newItem);
        }

        return result;
    }
}