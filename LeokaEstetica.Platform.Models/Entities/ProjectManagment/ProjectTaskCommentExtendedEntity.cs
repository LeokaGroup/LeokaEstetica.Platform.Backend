namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сущности расширяющей сущность комментариев задачи.
/// </summary>
public class ProjectTaskCommentExtendedEntity : ProjectTaskCommentEntity
{
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }
}