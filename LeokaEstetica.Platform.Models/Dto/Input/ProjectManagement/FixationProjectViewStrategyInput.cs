namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели фиксации выбранной пользователем стратегии представления.
/// </summary>
public class FixationProjectViewStrategyInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Системное название стратегии представления.
    /// </summary>
    public string StrategySysName { get; set; }
}