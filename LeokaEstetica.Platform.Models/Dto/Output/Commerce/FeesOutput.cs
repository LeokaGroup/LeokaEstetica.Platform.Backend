namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели услуги.
/// </summary>
public class FeesOutput
{
    /// <summary>
    /// Id услуги.
    /// </summary>
    public int FeesId { get; set; }

    /// <summary>
    /// Название услуги.
    /// </summary>
    public string? FeesName { get; set; }
    
    /// <summary>
    /// Системное название услуги.
    /// </summary>
    public string? FeesSysName { get; set; }

    /// <summary>
    /// Цена услуги. Может быть NULL (в таком кейсе значит бесплатно).
    /// </summary>
    public decimal? FeesPrice { get; set; }

    /// <summary>
    /// Ед.изм.
    /// </summary>
    public string? FeesMeasure { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int? FeesFareRuleId { get; set; }

    /// <summary>
    /// Признак активной услуги.
    /// </summary>
    public bool FeesIsActive { get; set; }
}