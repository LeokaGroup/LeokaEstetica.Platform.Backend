namespace LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

/// <summary>
/// Базовый класс статусов задач для разных типов.
/// </summary>
public class BaseTaskStatusOutput
{
    /// <summary>
    /// Id статуса.
    /// </summary>
    public int StatusId { get; set; }
    
    /// <summary>
    /// Id статуса, который маппится на статус задачи.
    /// </summary>
    public int TaskStatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string? StatusSysName { get; set; }
}