using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement.Paginator;

namespace LeokaEstetica.Platform.Models.Dto.Output.Template;

/// <summary>
/// Класс выходной модели статусов шаблонов задач.
/// </summary>
public class ProjectManagmentTaskStatusTemplateOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string? StatusSysName { get; set; }

    /// <summary>
    /// Id шаблона.
    /// Может быть NULL, например, для системных статусов (они в любом шаблоне).
    /// </summary>
    public long? TemplateId { get; set; }
    
    /// <summary>
    /// Id статуса, который маппится на статус задачи.
    /// </summary>
    public int TaskStatusId { get; set; }

    /// <summary>
    /// Список задач определенного статуса.
    /// </summary>
    public List<ProjectManagmentTaskOutput>? ProjectManagmentTasks { get; set; }

    /// <summary>
    /// Кол-во всего задач у статуса.
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// Класс пагинатора.
    /// </summary>
    public Paginator? Paginator { get; set; }

    /// <summary>
    /// Признак системного статуса. У него TemplateId всегда NULL.
    /// </summary>
    public bool IsSystemStatus { get; set; }
}