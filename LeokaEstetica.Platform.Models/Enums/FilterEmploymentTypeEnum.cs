using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров занятости.
/// </summary>
public enum FilterEmploymentTypeEnum
{
    [Description("Полная занятость")]
    Full = 1,
    
    [Description("Проектная работа")]
    ProjectWork = 2,
    
    [Description("Частичная занятость")]
    Partial = 3
}