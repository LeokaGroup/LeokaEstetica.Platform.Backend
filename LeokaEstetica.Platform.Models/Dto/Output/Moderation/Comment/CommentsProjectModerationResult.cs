namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Comment;

/// <summary>
/// Класс результата списка комментариев проекта.
/// </summary>
public class CommentsProjectModerationResult
{
    /// <summary>
    /// Список комментариев проекта.
    /// </summary>
    public IEnumerable<CommentProjectModerationOutput> Comments { get; set; }

    /// <summary>
    /// Всего комментариев проекта на модерации.
    /// </summary>
    public int Total => Comments.Count();
}
