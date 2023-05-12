using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Abstractions.Commerce;

/// <summary>
/// Абстракция репозитория заказов.
/// </summary>
public interface ICommerceRepository
{
    /// <summary>
    /// Метод создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderInput">Входная модель.</param>
    /// <returns>Данные заказа.</returns>
    Task<OrderEntity> CreateOrderAsync(CreatePaymentOrderInput createPaymentOrderInput);
}