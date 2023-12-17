using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Models.Dto.Output.Template;

/// <summary>
/// Класс результата шаблонов содержащих статусы. Эта модель нужна для данных из БД для маппинга в выходную модель.
/// </summary>
public class ProjectManagmentTaskTemplateEntityResult
{
    /// <summary>
    /// Название шаблона.
    /// </summary>
    public string TemplateName { get; set; }

    /// <summary>
    /// Список статусов шаблона.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusTemplateEntity> ProjectManagmentTaskStatusTemplates { get; set; }
}