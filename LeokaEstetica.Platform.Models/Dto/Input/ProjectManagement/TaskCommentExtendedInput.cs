namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс расширенной входной модели комментариев задачи.
/// </summary>
public class TaskCommentExtendedInput : TaskCommentInput
{
    /// <summary>
    /// Id комментария.
    /// </summary>
    public long CommentId { get; set; }
}