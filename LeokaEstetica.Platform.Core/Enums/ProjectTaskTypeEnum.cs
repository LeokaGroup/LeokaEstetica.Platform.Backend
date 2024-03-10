namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типов задач.
/// </summary>
public enum ProjectTaskTypeEnum
{
    /// <summary>
    /// Задача.
    /// </summary>
    Task = 1,
    
    /// <summary>
    /// Ошибка.
    /// </summary>
    Error = 2,
    
    /// <summary>
    /// История/требование.
    /// </summary>
    History = 3,
    
    /// <summary>
    /// Эпик.
    /// </summary>
    Epic = 4
}