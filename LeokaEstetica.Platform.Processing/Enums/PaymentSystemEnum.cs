using System.ComponentModel;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление типов платежных систем.
/// </summary>
public enum PaymentSystemEnum
{
    /// <summary>
    /// Платежная система ЮKassa.
    /// </summary>
    [Description("ЮKassa")]
    Yandex = 1,
    
    /// <summary>
    /// Платежная система PayMaster.
    /// </summary>
    [Description("PayMaster")]
    PayMaster = 2
}