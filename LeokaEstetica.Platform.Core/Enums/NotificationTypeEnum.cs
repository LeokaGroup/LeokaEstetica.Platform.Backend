using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типов уведомлений.
/// </summary>
public enum NotificationTypeEnum
{
    /// <summary>
    /// Приглашение в проект.
    /// </summary>
    [Description("Приглашение в проект")]
    ProjectInvite = 1,
    
    /// <summary>
    /// Одобрение проекта модератором.
    /// </summary>
    [Description("Одобрение проекта модератором")]
    ApproveModerationProject = 2,
    
    /// <summary>
    /// Отклонение проекта модератором.
    /// </summary>
    [Description("Отклонение проекта модератором")]
    RejectModerationProject = 3
}