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
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }
}