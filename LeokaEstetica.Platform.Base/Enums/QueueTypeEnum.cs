using System.ComponentModel;

namespace LeokaEstetica.Platform.Base.Enums;

/// <summary>
/// Перечисление типов очередей кролика.
/// </summary>
public enum QueueTypeEnum
{
    /// <summary>
    /// Очередь заказов.
    /// </summary>
    [Description("Orders.Queue")]
    OrdersQueue = 1,
    
    /// <summary>
    /// Очередь возвратов.
    /// </summary>
    [Description("Refunds.Queue")]
    RefundsQueue = 2,
    
    /// <summary>
    /// Очередь чеков возвратов.
    /// </summary>
    [Description("ReceiptRefund.Queue")]
    ReceiptRefundQueue = 3
}