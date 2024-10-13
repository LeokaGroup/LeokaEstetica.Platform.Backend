namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление модулей приложения.
/// </summary>
public enum UserConnectionModuleEnum
{
    /// <summary>
    /// Неизвестный тип.
    /// </summary>
    Undefined = 0,
    
    /// <summary>
    /// Основной модуль.
    /// </summary>
    Main = 1,
    
    /// <summary>
    /// Модуль УП - Управление проектами.
    /// </summary>
    ProjectManagement = 2,
    
    /// <summary>
    /// Модуль коммуникаций.
    /// </summary>
    Communications = 3
}