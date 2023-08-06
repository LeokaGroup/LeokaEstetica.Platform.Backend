namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов комментариев проекта на модерации.
/// </summary>
public enum ProjectCommentModerationEnum
{
    /// <summary>
    /// Опубликован.
    /// </summary>
    ApproveComment = 8,
    
    /// <summary>
    /// На модерации.
    /// </summary>
    ModerationComment = 9,
    
    /// <summary>
    /// Отклонен.
    /// </summary>
    RejectedComment = 10
}