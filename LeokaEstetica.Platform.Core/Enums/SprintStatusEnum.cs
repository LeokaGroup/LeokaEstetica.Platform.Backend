namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов спринта.
/// </summary>
public enum SprintStatusEnum
{
    /// <summary>
    /// В бэклоге.
    /// </summary>
    Backlog = 1,
    
    /// <summary>
    /// В работе.
    /// </summary>
    InWork = 2,
    
    /// <summary>
    /// Завершен.
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// Закрыт.
    /// </summary>
    Closed = 4,
    
    /// <summary>
    /// В архиве.
    /// </summary>
    Archive = 5
}