using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Receipts;

namespace LeokaEstetica.Platform.Base.Helpers;

/// <summary>
/// Класс наполнения данных события чека возврата.
/// </summary>
public static class ReceiptRefundEventFactory
{
    /// <summary>
    /// Метод наполняет данными событие возврата.
    /// </summary>
    /// <param name="receiptId">Id чека возврата.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="status">Статус возврата.</param>
    /// <returns>Результирующая модель.</returns>
    public static ReceiptRefundEvent CreateReceiptRefundEvent(string receiptId, string paymentId, string status)
    {
        return new ReceiptRefundEvent
        {
            ReceiptId = receiptId,
            PaymentId = paymentId,
            Status = status
        };
    }
}