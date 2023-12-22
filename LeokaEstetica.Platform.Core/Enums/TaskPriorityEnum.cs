using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление приоритета задачи.
/// </summary>
public enum TaskPriorityEnum
{
    /// <summary>
    /// Низкий.
    /// </summary>
    [Description("Низкий")]
    Low = 1,
    
    /// <summary>
    /// Средний.
    /// </summary>
    [Description("Средний")]
    Medium = 2,
    
    /// <summary>
    /// Высокий.
    /// </summary>
    [Description("Высокий")]
    High = 3,
    
    /// <summary>
    /// Срочный.
    /// </summary>
    [Description("Срочный")]
    Urgent = 4,
    
    /// <summary>
    /// Блокер.
    /// </summary>
    [Description("Блокер")]
    Blocker = 5
}