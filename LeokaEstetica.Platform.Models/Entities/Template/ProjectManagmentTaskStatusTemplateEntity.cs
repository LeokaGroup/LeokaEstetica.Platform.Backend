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
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// FK на шаблоны задач.
    /// </summary>
    public ProjectManagmentTaskTemplateEntity ProjectManagmentTaskTemplate { get; set; }
}