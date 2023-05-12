namespace LeokaEstetica.Platform.Models.Entities.FareRule;

/// <summary>
/// Класс сопоставляется с таблицей правил скидок Rules.DiscountRules.
/// </summary>
public class DiscountRuleEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Процент скидки.
    /// </summary>
    public decimal Percent { get; set; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Тип скидки.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Русское название.
    /// </summary>
    public string RussianName { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    public int Month { get; set; }
}