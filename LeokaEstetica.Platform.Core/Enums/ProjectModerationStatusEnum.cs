using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов модерации проектов.
/// </summary>
public enum ProjectModerationStatusEnum
{
    [Description("Одобрен")]
    ApproveProject = 4,
    
    [Description("На модерации")]
    ModerationProject = 2,
    
    [Description("Отклонен")]
    RejectedProject = 3
}