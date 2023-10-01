using LeokaEstetica.Platform.Base.Models.Dto;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.Receipts;

/// <summary>
/// Класс события чека возврата.
/// </summary>
public class ReceiptRefundEvent : BaseEventMessage
{
    /// <summary>
    /// Id чека в ПС.
    /// </summary>
    public string ReceiptId { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Статус чека в ПС.
    /// </summary>
    public string Status { get; set; }
}