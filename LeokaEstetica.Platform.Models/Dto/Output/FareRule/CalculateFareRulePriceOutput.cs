namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс выходной модели вычисления цены тарифа.
/// </summary>
public class CalculateFareRulePriceOutput
{
    /// <summary>
    /// Признак необходимости ожидания действий пользователя.
    /// </summary>
    public bool IsNeedUserAction { get; set; }
    
    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Price { get; set; }
}