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
    ReceiptRefundQueue = 1 << 4,
    
    /// <summary>
    /// Очередь сообщений чата нейросети.
    /// </summary>
    [Description("ScrumMasterAiMessage.Queue")]
    ScrumMasterAiMessage = 1 << 6,
    
    /// <summary>
    /// Очередь анализа чата нейросети.
    /// </summary>
    [Description("ScrumMasterAiAnalysis.Queue")]
    ScrumMasterAiAnalysis = 1 << 8,
    
    /// <summary>
    /// Очередь чата.
    /// </summary>
    [Description("DialogMessages.Queue")]
    DialogMessages = 1 << 10
}