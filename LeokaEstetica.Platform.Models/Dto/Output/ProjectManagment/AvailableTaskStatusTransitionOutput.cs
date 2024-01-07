namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели доступных переходов задачи.
/// </summary>
public class AvailableTaskStatusTransitionOutput
{
    /// <summary>
    /// Id статуса, который маппится на статус задачи.
    /// </summary>
    public int TaskStatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }
}