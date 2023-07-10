namespace LeokaEstetica.Platform.Models.Dto.Output.Metrics;

/// <summary>
/// Класс выходной модели последних комментариев к проектам.
/// </summary>
public class LastProjectCommentsOutput
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
    /// Дата создания комментария.
    /// </summary>
    public string Created { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }
}