namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Comment;

/// <summary>
/// Класс выходной модели комментария проекта для модерации.
/// </summary>
public class CommentProjectModerationOutput
{
    /// <summary>
    /// Id комментария.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Комментарий.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Дата модерации комментария.
    /// </summary>
    public string DateModeration { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public string DateCreated { get; set; }
}
