namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс описывает связи многие-ко-многим таблиц статусов.
/// </summary>
public class ProjectManagmentTaskStatusIntermediateTemplateEntity
{
    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ProjectManagmentTaskTemplateEntity ProjectManagmentTaskTemplate { get; set; }

    /// <summary>
    /// Id статуса.
    /// </summary>
    public long StatusId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ProjectManagmentTaskStatusTemplateEntity ProjectManagmentTaskStatusTemplate { get; set; }

    /// <summary>
    /// Признак кастомного статуса (если статус создал пользователь).
    /// </summary>
    public bool IsCustomStatus { get; set; }
}