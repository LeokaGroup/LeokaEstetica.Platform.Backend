using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Repositories.Commerce;

/// <summary>
/// Класс реализует методы репозитория заказов.
/// </summary>
public class CommerceRepository : ICommerceRepository
{
    private readonly PgContext _pgContext;

    public CommerceRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderInput">Входная модель.</param>
    /// <returns>Данные заказа.</returns>
    public async Task<OrderEntity> CreateOrderAsync(CreatePaymentOrderInput createPaymentOrderInput)
    {
        var order = new OrderEntity
        {
            UserId = createPaymentOrderInput.UserId,
            PaymentId = createPaymentOrderInput.PaymentId,
            OrderName = createPaymentOrderInput.Name,
            OrderDetails = createPaymentOrderInput.Description,
            Price = createPaymentOrderInput.Price,
            Currency = createPaymentOrderInput.Currency,
            DateCreated = createPaymentOrderInput.Created,
            PaymentMonth = createPaymentOrderInput.PaymentMonth,
            StatusName = createPaymentOrderInput.PaymentStatusName,
            StatusSysName = createPaymentOrderInput.PaymentStatusSysName
        };
        await _pgContext.Orders.AddAsync(order);
        await _pgContext.SaveChangesAsync();

        return order;
    }
}