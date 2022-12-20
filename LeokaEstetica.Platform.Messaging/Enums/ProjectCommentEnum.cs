using System.ComponentModel;

namespace LeokaEstetica.Platform.Messaging.Enums;

/// <summary>
/// Перечисление статусов комментариев к проектам.
/// </summary>
public enum ProjectCommentEnum
{
    /// <summary>
    /// Если отзыв к проекту одобрен.
    /// </summary>
    [Description("Одобрен")]
    Approve = 1,
    
    /// <summary>
    /// Если отзыв к проекту отклонен.
    /// </summary>
    [Description("Отклонен")]
    Rejected = 2,
    
    /// <summary>
    /// Если отзыв к проекту на модерации.
    /// </summary>
    [Description("На модерации")]
    Moderation = 3
}