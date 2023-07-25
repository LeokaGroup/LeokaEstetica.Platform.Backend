using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление типов лимитов понижения подписки.
/// </summary>
public enum ReductionSubscriptionLimitsEnum
{
    None = 1,
    
    /// <summary>
    /// Лимиты на проекты.
    /// </summary>
    [Description("Лимиты на кол-во проектов")]
    Project = 2,
    
    /// <summary>
    /// Лимиты на вакансии.
    /// </summary>
    [Description("Лимиты на кол-во вакансий")]
    Vacancy = 3
}