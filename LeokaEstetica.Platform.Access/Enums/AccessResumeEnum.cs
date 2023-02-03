using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление типов доступов.
/// </summary>
public enum AccessResumeEnum
{
    [Description("Нет доступа.")]
    NotAvailable = 0,
    
    [Description("Базовый тариф.")]
    Base = 1,
    
    [Description("Тариф бизнес.")]
    Business = 2,
    
    [Description("Тариф профессиональный.")]
    Professional = 3
}