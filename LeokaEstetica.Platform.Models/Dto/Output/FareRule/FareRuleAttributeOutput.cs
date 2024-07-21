namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс выходной модели атрибутов тарифа.
/// </summary>
public class FareRuleAttributeOutput
{
    /// <summary>
    /// Id атрибута.
    /// </summary>
    public int AttributeId { get; set; }

    /// <summary>
    /// Название атрибута.
    /// </summary>
    public string? AttributeName { get; set; }

    /// <summary>
    /// Описание атрибута.
    /// </summary>
    public string? AttributeDetails { get; set; }
    
    /// <summary>
    /// Значения атрибута.
    /// </summary>
    public IEnumerable<FareRuleAttributeValueOutput>? FareRuleAttributeValues { get; set; }
}