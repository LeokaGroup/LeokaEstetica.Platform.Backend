namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс сопоставляется с таблицей шаблонов задач.
/// </summary>
public class ProjectManagmentTaskTemplateEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Название шаблона.
    /// </summary>
    public string TemplateName { get; set; }

    /// <summary>
    /// Системное название шаблона.
    /// </summary>
    public string TemplateSysName { get; set; }

    /// <summary>
    /// Список статусов шаблонов задач.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusTemplateEntity> ProjectManagmentTaskStatusTemplates { get; set; }

    //
    // /// <summary>
    // /// Список статусов шаблонов задач пользователя.
    // /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusIntermediateTemplateEntity>
        ProjectManagmentTaskStatusIntermediateTemplates { get; set; }
}