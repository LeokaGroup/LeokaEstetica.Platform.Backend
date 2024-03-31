namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов переходов статусов.
/// </summary>
public enum TransitionTypeEnum
{
    None = 0,
    
    /// <summary>
    /// Задача.
    /// </summary>
    Task = 1,
    
    /// <summary>
    /// Эпик.
    /// </summary>
    Epic = 2,
    
    /// <summary>
    /// История.
    /// </summary>
    History = 3,
    
    /// <summary>
    /// Спринт.
    /// </summary>
    Sprint = 4
}