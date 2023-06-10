using LeokaEstetica.Platform.Base.Models.Dto;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.Refunds;

/// <summary>
/// Класс события возврата.
/// </summary>
public class RefundEvent : BaseEventMessage
{
    /// <summary>
    /// PK.
    /// </summary>
    public long RefundId { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Статус возврата.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    public string RefundOrderId { get; set; }
}