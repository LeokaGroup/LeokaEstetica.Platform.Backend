using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов модерации комментариев проекта.
/// </summary>
public enum CommentModerationStatusEnum
{
    /// <summary>
    /// Комментарий одобрен.
    /// </summary>
    [Description("Одобрен")]
    ApproveComment = 8,

    /// <summary>
    /// Комментарий на модерации.
    /// </summary>
    [Description("На модерации")]
    ModerationComment = 9,

    /// <summary>
    /// Комментарий отклонен.
    /// </summary>
    [Description("Отклонен")]
    RejectedComment = 10,
}
