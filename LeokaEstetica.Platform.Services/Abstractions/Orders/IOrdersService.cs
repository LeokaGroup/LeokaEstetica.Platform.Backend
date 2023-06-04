using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Services.Abstractions.Orders;

/// <summary>
/// Абстракция сервиса заказов.
/// </summary>
public interface IOrdersService
{
    /// <summary>
    /// Метод получает список заказов пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список заказов пользователя.</returns>
    Task<IEnumerable<OrderEntity>> GetUserOrdersAsync(string account);

    /// <summary>
    /// Метод получает детали заказа по его Id.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Детали заказа.</returns>
    Task<OrderEntity> GetOrderDetailsAsync(long orderId, string account);

    /// <summary>
    /// Метод получает список транзакций по заказам пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список транзакций.</returns>
    Task<IEnumerable<HistoryEntity>> GetHistoryAsync(string account);
}