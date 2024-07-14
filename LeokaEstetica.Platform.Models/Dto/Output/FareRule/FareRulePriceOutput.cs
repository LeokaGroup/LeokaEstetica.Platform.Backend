namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс выходной модели цен тарифа.
/// </summary>
public class FareRulePriceOutput
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
}