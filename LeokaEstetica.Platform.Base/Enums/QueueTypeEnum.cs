using System.ComponentModel;

namespace LeokaEstetica.Platform.Base.Enums;

/// <summary>
/// Перечисление типов очередей кролика.
/// </summary>
[Flags]
public enum QueueTypeEnum
{
    /// <summary>
    /// Очередь заказов.
    /// </summary>
    [Description("Orders.Queue")]
    OrdersQueue = 1 << 0,
    
    /// <summary>
    /// Очередь возвратов.
    /// </summary>
    [Description("Refunds.Queue")]
    RefundsQueue = 1 << 2,
    
    /// <summary>
    /// Очередь чеков возвратов.
    /// </summary>
    [Description("ReceiptRefund.Queue")]
    ReceiptRefundQueue = 1 << 4
}