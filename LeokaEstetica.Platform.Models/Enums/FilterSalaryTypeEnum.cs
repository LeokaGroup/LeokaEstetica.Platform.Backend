using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров по соответствиям.
/// </summary>
public enum FilterSalaryTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("По соответствию")]
    Match = 2,
    
    [Description("По дате")]
    Date = 3,
    
    [Description("По убыванию зарплат")]
    DescSalary = 4,
    
    [Description("По возрастанию зарплат")]
    AscSalary = 5
}