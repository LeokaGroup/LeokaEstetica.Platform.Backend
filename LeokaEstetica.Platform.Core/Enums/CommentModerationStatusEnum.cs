using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов модерации комментариев проекта.
/// </summary>
public enum CommentModerationStatusEnum
{
    [Description("Одобрен")]
    ApproveComment = 8,

    [Description("На модерации")]
    ModerationComment = 9,

    [Description("Отклонен")]
    RejectedComment = 10,
}
