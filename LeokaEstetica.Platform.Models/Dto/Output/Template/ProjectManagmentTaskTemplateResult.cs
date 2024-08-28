namespace LeokaEstetica.Platform.Models.Dto.Output.Template;

/// <summary>
/// Класс результата шаблонов содержащих статусы.
/// </summary>
public class ProjectManagmentTaskTemplateResult
{
    /// <summary>
    /// Название шаблона.
    /// </summary>
    public string? TemplateName { get; set; }

    /// <summary>
    /// Список статусов шаблона.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusTemplateOutput>? ProjectManagmentTaskStatusTemplates { get; set; }
}