namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей стратегий представления рабочего пространства проекта.
/// </summary>
public class ViewStrategyEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StrategyId { get; set; }

    /// <summary>
    /// Название стратегии представления.
    /// </summary>
    public string ViewStrategyName { get; set; }

    /// <summary>
    /// Системное название стратегии представления.
    /// </summary>
    public string ViewStrategySysName { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }
}