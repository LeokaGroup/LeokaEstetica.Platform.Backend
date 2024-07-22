namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс композитной выходной модели полученного тарифа вместе с его атрибутами.
/// </summary>
public class FareRuleAttributeCompositeOutput
{
    /// <summary>
    /// Минимальное значение цены.
    /// </summary>
    public decimal? MinValue { get; set; }
    
    /// <summary>
    /// Максимальное значение цены.
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string? FareRuleName { get; set; }
    
    /// <summary>
    /// Публичный код для отображения в строке URL.
    /// </summary>
    public Guid PublicId { get; }
    
    /// <summary>
    /// Позиция в списке.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Признак бесплатного тарифа.
    /// </summary>
    public bool IsFree { get; set; }
}