using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров занятости.
/// </summary>
public enum FilterEmploymentTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("Полная занятость")]
    Full = 2,
    
    [Description("Проектная работа")]
    ProjectWork = 3,
    
    [Description("Частичная занятость")]
    Partial = 4
}