namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс сопоставляется с таблицей переходов статусов шаблонов пользователя.
/// </summary>
public class ProjectManagementUserTransitionTemplateEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long TransitionId { get; set; }

    /// <summary>
    /// Название перехода.
    /// </summary>
    public string TransitionName { get; set; }

    /// <summary>
    /// Системное название перехода.
    /// </summary>
    public string TransitionSysName { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Id статуса, из которого переход.
    /// </summary>
    public long FromStatusId { get; set; }

    /// <summary>
    /// Id статуса, в который переход.
    /// </summary>
    public long ToStatusId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}