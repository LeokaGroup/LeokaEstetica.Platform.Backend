using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление типов лимитов понижения подписки.
/// </summary>
public enum ReductionSubscriptionLimitsEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Лимиты на кол-во сотрудников в тарифе.
    /// </summary>
    [Description("Лимиты на кол-во сотрудников")]
    EmployeesCount = 2
}