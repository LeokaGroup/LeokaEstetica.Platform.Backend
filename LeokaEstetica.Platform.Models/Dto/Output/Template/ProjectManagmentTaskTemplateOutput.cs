using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Models.Dto.Output.Template;

/// <summary>
/// Класс выходной модели шаблонов задач.
/// </summary>
public class ProjectManagmentTaskTemplateOutput
{
    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }

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
}