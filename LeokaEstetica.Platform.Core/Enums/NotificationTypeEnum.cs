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
    RejectModerationProject = 3,
    
    /// <summary>
    /// Одобрение вакансии модератором.
    /// </summary>
    [Description("Одобрение вакансии модератором")]
    ApproveModerationVacancy = 4,
    
    /// <summary>
    /// Отклонение вакансии модератором.
    /// </summary>
    [Description("Отклонение вакансии модератором")]
    RejectModerationVacancy = 5,
    
    /// <summary>
    /// Принятие приглашения в проект.
    /// </summary>
    [Description("Принятие приглашения в проект")]
    ApproveInviteProject = 6,
    
    /// <summary>
    /// Отклонение приглашения в проект.
    /// </summary>
    [Description("Отклонение приглашения в проект")]
    RejectInviteProject = 7
}