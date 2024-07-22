namespace LeokaEstetica.Platform.Models.Dto.Output.Subscription;

/// <summary>
/// Класс композитной выходной модели подписок и тарифов.
/// </summary>
public class SubscriptionFareRuleCompositeOutput
{
    /// <summary>
    /// Признак бесплатного тарифа.
    /// </summary>
    public bool IsFree { get; set; }
    
    /// <summary>
    /// Минимальное значение цены.
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// Признак наличия цены тарифа.
    /// </summary>
    public bool IsPrice { get; set; }
    
    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }
}