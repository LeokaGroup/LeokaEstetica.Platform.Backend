namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели комментария задачи.
/// </summary>
public class TaskCommentInput
{
    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public string ProjectTaskId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Комментарий к задаче.
    /// </summary>
    public string Comment { get; set; }
}