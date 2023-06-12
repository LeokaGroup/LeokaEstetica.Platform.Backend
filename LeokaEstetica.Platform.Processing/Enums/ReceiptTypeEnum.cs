using System.ComponentModel;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление типов чека.
/// </summary>
public enum ReceiptTypeEnum
{
    /// <summary>
    /// Тип для чека при оплате заказа.
    /// </summary>
    [Description("Приход")]
    Payment = 1,
    
    /// <summary>
    /// Тип для чека при возврате заказа.
    /// </summary>
    [Description("Возврат прихода")]
    Refund = 2
}