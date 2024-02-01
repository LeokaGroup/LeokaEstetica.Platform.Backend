namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов связей задач.
/// </summary>
public enum LinkTypeEnum
{
    /// <summary>
    /// Обычная связь (связана с).
    /// </summary>
    Link = 1,
    
    /// <summary>
    /// Родитель.
    /// </summary>
    Parent = 2,
    
    /// <summary>
    /// Дочка.
    /// </summary>
    Child = 3,
    
    /// <summary>
    /// Зависит от.
    /// </summary>
    Depend = 4
}