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

    #region Публичные методы.

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

    /// <summary>
    /// Метод получает детали заказа по его Id.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Детали заказа.</returns>
    public async Task<OrderEntity> GetOrderDetailsAsync(long orderId, long userId)
    {
        var result = await _pgContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId
                                                                      && o.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод получает список транзакций по заказам пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список транзакций.</returns>
    public async Task<IEnumerable<HistoryEntity>> GetHistoryAsync(long userId)
    {
        var result = await _pgContext.OrderTransactionsShadow
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}