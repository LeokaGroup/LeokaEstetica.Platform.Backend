namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели оставления комментария к проекту.
/// </summary>
public class ProjectCommentInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Comment { get; set; }
}