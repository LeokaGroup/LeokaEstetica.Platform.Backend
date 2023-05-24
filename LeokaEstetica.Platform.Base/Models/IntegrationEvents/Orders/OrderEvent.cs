using LeokaEstetica.Platform.Base.Models.Dto;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;

/// <summary>
/// Класс события заказа.
/// </summary>
public class OrderEvent : BaseEventMessage
{
    /// <summary>
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }
    
    /// <summary>
    /// Системное название статуса заказа.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }
}