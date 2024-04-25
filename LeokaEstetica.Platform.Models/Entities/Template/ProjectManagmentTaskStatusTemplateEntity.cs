namespace LeokaEstetica.Platform.Models.Entities.Template;

/// <summary>
/// Класс сопоставляется с таблицей шаблонов статусов задач.
/// </summary>
public class ProjectManagmentTaskStatusTemplateEntity
{
    public ProjectManagmentTaskStatusTemplateEntity(string statusName, string statusSysName)
    {
        StatusName = statusName;
        StatusSysName = statusSysName;
    }
    /// <summary>
    /// PK.
    /// </summary>
    public long StatusId { get; set; }

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
}