using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление статусов откликов на проекты.
/// </summary>
public enum ProjectResponseStatusEnum
{
    /// <summary>
    /// Если статус в ожидании ответа.
    /// </summary>
    [Description("Ожидает ответа")]
    Wait = 1,
    
    /// <summary>
    /// Если статус принят.
    /// </summary>
    [Description("Принят")]
    Approved = 2,
    
    /// <summary>
    /// Если статус отклонен.
    /// </summary>
    [Description("Отклонен")]
    Rejected = 6,
    
    /// <summary>
    /// Если статус отклонен модератором.
    /// </summary>
    [Description("Отклонен модератором")]
    RejectedModerator = 4
}