namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типа компонента, результаты которого нужно модифицировать.
/// </summary>
public enum ModifyTaskStatuseTypeEnum
{
    /// <summary>
    /// Рабочее пространство.
    /// </summary>
    Space = 1,
    
    /// <summary>
    /// Бэклог.
    /// </summary>
    Backlog = 2,
    
    /// <summary>
    /// Спринт.
    /// </summary>
    Sprint = 3
}