using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Commerce;

/// <summary>
/// TODO: Отрефачить разбив логику заказов в отдельный репозиторий OrderRepository.
/// Класс реализует методы репозитория коммерции.
/// </summary>
public class CommerceRepository : ICommerceRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public CommerceRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

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

    /// <summary>
    /// Метод получает скидку на услугу по ее типу и кол-ву месяцев.
    /// </summary>
    /// <param name="paymentMonth">Кол-во месяцев.</param>
    /// <param name="discountTypeEnum">Тип скидки на услугу</param>
    /// <returns>Скидка на услугу.</returns>
    public async Task<decimal> GetPercentDiscountAsync(short paymentMonth, DiscountTypeEnum discountTypeEnum)
    {
        var result = await _pgContext.DiscountRules
            .Where(d => d.Month == paymentMonth
                        && d.Type.Equals(discountTypeEnum.ToString()))
            .Select(d => Math.Round(d.Percent))
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет статус заказа.
    /// </summary>
    /// <param name="paymentStatusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="orderId">Id заказа в БД.</param>
    public async Task<bool> UpdateOrderStatusAsync(string paymentStatusSysName, string paymentId, long orderId)
    {
        var updateOrder = await _pgContext.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId 
                                      && o.PaymentId.Equals(paymentId));

        if (updateOrder is null)
        {
            return false;
        }

        updateOrder.StatusSysName = paymentStatusSysName;
        await _pgContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}