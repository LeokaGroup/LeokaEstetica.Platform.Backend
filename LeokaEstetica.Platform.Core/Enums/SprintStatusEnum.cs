using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов спринта.
/// </summary>
public enum SprintStatusEnum
{
    /// <summary>
    /// Новый.
    /// </summary>
    [Description("Новый")]
    New = 1,
    
    /// <summary>
    /// В работе.
    /// </summary>
    [Description("В работе")]
    InWork = 2,
    
    /// <summary>
    /// Завершен.
    /// </summary>
    [Description("Завершен")]
    Completed = 3,
    
    /// <summary>
    /// Закрыт.
    /// </summary>
    [Description("Закрыт")]
    Closed = 4
}