using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Orders;

/// <summary>
/// Класс реализует методы репозитория заказов.
/// </summary>
public class OrdersRepository : IOrdersRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public OrdersRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список заказов пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список заказов пользователя.</returns>
    public async Task<IEnumerable<OrderEntity>> GetUserOrdersAsync(long userId)
    {
        var result = await _pgContext.Orders
            .Where(o => o.UserId == userId)
            .Select(o => new OrderEntity
            {
                OrderId = o.OrderId,
                OrderName = o.OrderName,
                OrderDetails = o.OrderDetails,
                Price = o.Price,
                DateCreated = o.DateCreated,
                StatusName = o.StatusName
            })
            .OrderByDescending(o => o.DateCreated)
            .ToListAsync();

        return result;
    }
}