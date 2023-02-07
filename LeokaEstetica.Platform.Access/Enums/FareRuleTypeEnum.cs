using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление типов подписок.
/// </summary>
public enum FareRuleTypeEnum
{
    [Description("Нет доступа.")]
    NotAvailable = 0,
    
    [Description("Тариф старт.")]
    Start = 1,
    
    [Description("Базовый тариф.")]
    Base = 2,
    
    [Description("Тариф бизнес.")]
    Business = 3,
    
    [Description("Тариф профессиональный.")]
    Professional = 4
}