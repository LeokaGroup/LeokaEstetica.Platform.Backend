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
    OrdersQueue = 1
}