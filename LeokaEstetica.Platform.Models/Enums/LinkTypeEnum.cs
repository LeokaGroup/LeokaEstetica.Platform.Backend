using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов связей задач.
/// </summary>
public enum LinkTypeEnum
{
    /// <summary>
    /// Обычная связь (связана с).
    /// </summary>
    [Description("Связана с")]
    Link = 1,
    
    /// <summary>
    /// Родитель.
    /// </summary>
    [Description("Родительская связь")]
    Parent = 2,
    
    /// <summary>
    /// Дочка.
    /// </summary>
    [Description("Дочерняя связь")]
    Child = 3,
    
    /// <summary>
    /// Зависит от.
    /// </summary>
    [Description("Зависит от")]
    Depend = 4
}