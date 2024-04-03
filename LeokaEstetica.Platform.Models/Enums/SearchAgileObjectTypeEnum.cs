namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов поиска Agile-объектов.
/// Под Agile-объектом подразумевается задача, ошибка, эпик, история, спринт.
/// </summary>
public enum SearchAgileObjectTypeEnum
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
    /// Эпик.
    /// </summary>
    Epic = 3,
    
    /// <summary>
    /// История.
    /// </summary>
    Story = 4,
    
    /// <summary>
    /// Спринт.
    /// </summary>
    Sprint = 5
}