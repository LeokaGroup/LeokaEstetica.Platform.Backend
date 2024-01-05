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
    /// Список переходов статусов шаблонов.
    /// </summary>
    public IEnumerable<ProjectManagementTransitionTemplateEntity> ProjectManagementTransitionTemplates { get; set; }

    /// <summary>
    /// Список переходов статусов шаблонов пользователей.
    /// </summary>
    public IEnumerable<ProjectManagementUserTransitionTemplateEntity> ProjectManagementUserTransitionTemplates { get; set; }
}