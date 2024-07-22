namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс выходной модели значений атрибутов тарифа.
/// </summary>
public class FareRuleAttributeValueOutput
{
    /// <summary>
    /// Id значения.
    /// </summary>
    public int ValueId { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Признак наличия цены.
    /// </summary>
    public bool IsPrice { get; set; }
    
    /// <summary>
    /// Id атрибута.
    /// </summary>
    public int AttributeId { get; set; }

    /// <summary>
    /// Ед.изм.
    /// </summary>
    public string? Measure { get; set; }

    /// <summary>
    /// Минимальное значение.
    /// </summary>
    public decimal? MinValue { get; set; }
    
    /// <summary>
    /// Максимальное значение.
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// Признак наличия диапазона значений.
    /// </summary>
    public bool IsRange { get; set; }

    /// <summary>
    /// Пояснение к контенту атрибута.
    /// </summary>
    public string? ContentTooltip { get; set; }

    /// <summary>
    /// Контент атрибута.
    /// </summary>
    public string? Content { get; set; }
}