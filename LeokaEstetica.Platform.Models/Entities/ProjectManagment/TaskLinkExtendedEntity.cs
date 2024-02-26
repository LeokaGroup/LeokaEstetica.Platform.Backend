namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сущности расширяющий сущность связей задач.
/// </summary>
public class TaskLinkExtendedEntity : TaskLinkEntity
{
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }
    
    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ToTaskId);
    
    /// <summary>
    /// Id задачи вместе с префиксом.
    /// </summary>
    public string FullTaskId => string.Concat(TaskIdPrefix + "-", ToTaskId);
    
    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullBlockedProjectTaskId => string.Concat(TaskIdPrefix + "-", BlockedTaskId);
    
    /// <summary>
    /// Id задачи вместе с префиксом.
    /// </summary>
    public string FullBlockedTaskId => string.Concat(TaskIdPrefix + "-", BlockedTaskId);
}