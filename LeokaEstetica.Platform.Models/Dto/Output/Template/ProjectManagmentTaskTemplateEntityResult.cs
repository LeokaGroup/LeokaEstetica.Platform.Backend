using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Models.Dto.Output.Template;

/// <summary>
/// Класс результата шаблонов содержащих статусы. Эта модель нужна для данных из БД для маппинга в выходную модель.
/// </summary>
public class ProjectManagmentTaskTemplateEntityResult : ProjectManagmentTaskStatusTemplateEntity
{
    /// <summary>
    /// Название шаблона.
    /// </summary>
    public string? TemplateName { get; set; }

    /// <summary>
    /// Id шаблона.
    /// </summary>
    public long? TemplateId { get; set; }
    
    /// <summary>
    /// Признак системного статуса. У него TemplateId всегда NULL.
    /// </summary>
    public bool IsSystemStatus { get; set; }
}