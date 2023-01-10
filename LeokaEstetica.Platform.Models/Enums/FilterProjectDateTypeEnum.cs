using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров дат.
/// </summary>
public enum FilterProjectDateTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("По дате.")]
    Date = 2
}