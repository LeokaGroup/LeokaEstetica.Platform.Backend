namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс композитной выходной модели для агрегации тарифов из разных модулей.
/// </summary>
public class FareRuleCompositeOutput
{
    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string? RuleName { get; set; }

    /// <summary>
    /// Признак бесплатного тарифа.
    /// </summary>
    public bool IsFree { get; set; }

    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Атрибуты тарифа.
    /// </summary>
    public IEnumerable<FareRuleAttributeOutput>? FareRuleAttributes { get; set; }
}