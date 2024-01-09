using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели доступных переходов задачи.
/// </summary>
public class AvailableTaskStatusTransitionOutput
{
    /// <summary>
    /// Id доступного к переходу статуса.
    /// </summary>
    public long AvailableStatusId { get; set; }
    
    /// <summary>
    /// Название доступного к переходу статуса.
    /// </summary>
    public string AvailableStatusName { get; set; }

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
}