namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типов детализации.
/// </summary>
public enum TaskDetailTypeEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    None = 0,
    
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
    Epic = 4,
    
    /// <summary>
    /// Спринт.
    /// </summary>
    Sprint = 5
}