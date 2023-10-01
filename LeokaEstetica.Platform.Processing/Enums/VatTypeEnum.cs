using System.ComponentModel;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление ставок НДС.
/// </summary>
public enum VatTypeEnum
{
    [Description("Без НДС")]
    None = 1,
    
    [Description("НДС 0%")]
    Vat0 = 2,
    
    [Description("НДС 10%")]
    Vat10 = 3,
    
    [Description("НДС 20%")]
    Vat20 = 4,
    
    [Description("НДС по формуле 10/110")]
    Vat110 = 5,
    
    [Description("НДС по формуле 20/120")]
    Vat120 = 6
}