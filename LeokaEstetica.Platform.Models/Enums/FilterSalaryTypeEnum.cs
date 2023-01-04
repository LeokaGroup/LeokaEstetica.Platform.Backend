using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов фильтров по соответствиям.
/// Используется при фильтрации по соответствиям.
/// </summary>
public enum FilterSalaryTypeEnum
{
    [Description("По соответствию")]
    Match = 1,
    
    [Description("По дате")]
    Date = 2,
    
    [Description("По убыванию зарплат")]
    DescSalary = 3,
    
    [Description("По возрастанию зарплат")]
    AscSalary = 4
}