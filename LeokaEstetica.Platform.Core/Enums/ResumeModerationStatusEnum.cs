using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов модерации анкет пользователей.
/// </summary>
public enum ResumeModerationStatusEnum
{
    [Description("Одобрена")]
    ApproveResume = 1,
    
    [Description("На модерации")]
    ModerationResume = 2,
    
    [Description("Отклонена")]
    RejectedResume = 3
}