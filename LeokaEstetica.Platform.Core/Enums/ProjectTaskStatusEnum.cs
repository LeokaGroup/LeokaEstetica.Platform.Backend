namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов задач.
/// </summary>
public enum ProjectTaskStatusEnum
{
    /// <summary>
    /// Новая.
    /// </summary>
    New = 1,
    
    /// <summary>
    /// Или Resolved тоже = 6. Готово, решена.
    /// </summary>
    Completed = 6,
    
    /// <summary>
    /// Архив.
    /// </summary>
    Archive = 7,
    
    /// <summary>
    /// Закрыта.
    /// </summary>
    Closed = 8
}