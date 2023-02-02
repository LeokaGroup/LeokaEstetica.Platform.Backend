using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление типов доступов.
/// </summary>
public enum AccessResumeEnum
{
    /// <summary>
    /// Пользователь не имеет доступа.
    /// </summary>
    [Description("Нет доступа.")]
    NotAvailable = 0,
    
    /// <summary>
    /// Пользователь имеет базовый доступ.
    /// </summary>
    [Description("Базовый доступ.")]
    Standart = 1
}