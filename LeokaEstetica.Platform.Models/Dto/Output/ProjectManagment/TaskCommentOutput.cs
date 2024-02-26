using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели комментария задачи.
/// </summary>
public class TaskCommentOutput : ProjectTaskCommentEntity
{
    /// <summary>
    /// Префикс.
    /// </summary>
    public string TaskIdPrefix { get; set; }
    
    /// <summary>
    /// Id задачи, которой принадлежит комментарий с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ProjectTaskId);
}