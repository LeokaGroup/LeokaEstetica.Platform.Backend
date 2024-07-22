namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типов детализации.
/// </summary>
public enum TaskDetailTypeEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Ошибка.
    /// </summary>
    Error = 1,
    
    /// <summary>
    /// Задача.
    /// </summary>
    Task = 3,

    /// <summary>
    /// История/требование.
    /// </summary>
    Story = 2,
    
    /// <summary>
    /// Эпик.
    /// </summary>
    Epic = 4,
    
    /// <summary>
    /// Спринт.
    /// </summary>
    Sprint = 6
}