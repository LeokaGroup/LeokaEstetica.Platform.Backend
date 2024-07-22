namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов поиска Agile-объектов.
/// Под Agile-объектом подразумевается задача, ошибка, эпик, история, спринт.
/// </summary>
public enum SearchAgileObjectTypeEnum
{
    /// <summary>
    /// Ошибка.
    /// </summary>
    Error = 1,
    
    /// <summary>
    /// Задача.
    /// </summary>
    Task = 3,

    /// <summary>
    /// Эпик.
    /// </summary>
    Epic = 4,
    
    /// <summary>
    /// История.
    /// </summary>
    Story = 2
}