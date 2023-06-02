using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Abstractions.Orders;

/// <summary>
/// Абстракция репозитория заказов.
/// </summary>
public interface IOrdersRepository
{
    /// <summary>
    /// Метод получает список заказов пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список заказов пользователя.</returns>
    Task<IEnumerable<OrderEntity>> GetUserOrdersAsync(long userId);

    /// <summary>
    /// Метод получает детали заказа по его Id.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Детали заказа.</returns>
    Task<OrderEntity> GetOrderDetailsAsync(long orderId, long userId);

    /// <summary>
    /// Метод получает список транзакций по заказам пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список транзакций.</returns>
    Task<IEnumerable<HistoryEntity>> GetHistoryAsync(long userId);
}