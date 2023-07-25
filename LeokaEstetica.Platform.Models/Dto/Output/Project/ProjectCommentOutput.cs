namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели комментариев проекта.
/// </summary>
public class ProjectCommentOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Признак принадлежности комментария текущему пользователю.
    /// </summary>
    public bool IsMyComment { get; set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public string Created { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}