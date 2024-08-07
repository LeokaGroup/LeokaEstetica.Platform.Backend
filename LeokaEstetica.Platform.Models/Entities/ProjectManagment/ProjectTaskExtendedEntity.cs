namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сущности расширяющей сущность задачи.
/// </summary>
public class ProjectTaskExtendedEntity : ProjectTaskEntity
{
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }
    
    /// <summary>
    /// Полное название задачи.
    /// </summary>
    public string NameTooltip { get; set; }
}