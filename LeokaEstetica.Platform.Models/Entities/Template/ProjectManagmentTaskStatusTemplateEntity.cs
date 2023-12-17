namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс сопоставляется с таблицей шаблонов статусов задач.
/// </summary>
public class ProjectManagmentTaskStatusTemplateEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Id статуса, который маппится на статус задачи.
    /// </summary>
    public int TaskStatusId { get; set; }

    /// <summary>
    /// FK многие-ко-многим на шаблоны задач.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskTemplateEntity> ProjectManagmentTaskTemplates { get; set; }

    /// <summary>
    /// Список статусов.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusIntermediateTemplateEntity>
        ProjectManagmentTaskStatusIntermediateTemplates { get; set; }
}