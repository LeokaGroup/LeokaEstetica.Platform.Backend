using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление типов подписок.
/// </summary>
public enum FareRuleTypeEnum
{
    [Description("Нет доступа.")]
    NotAvailable = 0,
    
    [Description("Старт")]
    Start = 1,
    
    [Description("Базовый")]
    Base = 2,
    
    [Description("Бизнес")]
    Business = 3,
    
    [Description("Профессиональный")]
    Professional = 4
}