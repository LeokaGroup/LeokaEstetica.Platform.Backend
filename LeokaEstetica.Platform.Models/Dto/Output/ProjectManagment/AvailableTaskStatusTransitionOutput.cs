using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели доступных переходов задачи.
/// </summary>
public class AvailableTaskStatusTransitionOutput
{
    /// <summary>
    /// Id доступного к переходу статуса.
    /// </summary>
    public long StatusId { get; set; }
    
    /// <summary>
    /// Название доступного к переходу статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название доступного к переходу статуса.
    /// </summary>
    [JsonIgnore]
    public string AvailableStatusSysName { get; set; }
    
    /// <summary>
    /// Id шаблона.
    /// </summary>
    [JsonIgnore]
    public int TemplateId { get; set; }

    /// <summary>
    /// Id статуса задачи, на которым мапится StatusId.
    /// </summary>
    public long TaskStatusId { get; set; }
}