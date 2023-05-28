using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;

namespace LeokaEstetica.Platform.Base.Helpers;

/// <summary>
/// Класс наполнения данных события заказов.
/// </summary>
public static class OrderEventFactory
{
    /// <summary>
    /// Метод наполняет данными событие заказа.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="statusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <returns>Результирующая модель.</returns>
    public static OrderEvent CreateOrderEvent(long orderId, string statusSysName, string paymentId)
    {
        return new OrderEvent
        {
            OrderId = orderId,
            StatusSysName = statusSysName,
            PaymentId = paymentId
        };
    }
}