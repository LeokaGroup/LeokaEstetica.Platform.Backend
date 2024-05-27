using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Refunds;

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Класс наполнения данных события возвратов.
/// </summary>
public static class RefundEventFactory
{
    /// <summary>
    /// Метод наполняет данными событие возврата.
    /// </summary>
    /// <param name="refundId">Id возврата.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="status">Статус возврата.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <returns>Результирующая модель.</returns>
    public static RefundEvent CreateRefundEvent(long refundId, string paymentId, string status, string refundOrderId)
    {
        return new RefundEvent
        {
            RefundId = refundId,
            PaymentId = paymentId,
            Status = status,
            RefundOrderId = refundOrderId
        };
    }
}