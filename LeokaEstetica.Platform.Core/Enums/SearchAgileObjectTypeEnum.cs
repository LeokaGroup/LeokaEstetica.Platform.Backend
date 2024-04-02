namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типа поиска Agile-объекта.
/// </summary>
public enum SearchAgileObjectTypeEnum
{
    /// <summary>
    /// Поиск задачи.
    /// </summary>
    Task = 1,
    
    /// <summary>
    /// Поиск эпика.
    /// </summary>
    Epic = 2,
    
    /// <summary>
    /// Поиск истории.
    /// </summary>
    Story = 3,
    
    /// <summary>
    /// Поиск ошибки.
    /// </summary>
    Error = 4,
    
    /// <summary>
    /// Поиск спринта.
    /// </summary>
    Sprint = 5
}