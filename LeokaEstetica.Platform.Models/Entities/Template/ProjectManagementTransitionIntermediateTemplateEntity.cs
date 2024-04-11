namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс описывает связь многие-многие таблиц переходов статусов.
/// </summary>
public class ProjectManagementTransitionIntermediateTemplateEntity
{
    /// <summary>
    /// Id перехода.
    /// </summary>
    public long TransitionId { get; set; }
    
    /// <summary>
    /// Id статуса, из которого переход.
    /// </summary>
    public long FromStatusId { get; set; }

    /// <summary>
    /// Id статуса, в который переход.
    /// </summary>
    public long ToStatusId { get; set; }

    /// <summary>
    /// Признак кастомного перехода (если он создавался пользователем).
    /// </summary>
    public bool IsCustomTransition { get; set; }
}