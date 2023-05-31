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
}