namespace LeokaEstetica.Platform.Core.Structs;

/// <summary>
/// Структура описывает скидки.
/// </summary>
public struct DiscountStruct
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="percent">% скидки.</param>
    /// <param name="price">Сумма скидки.</param>
    public DiscountStruct(decimal percent, decimal? price)
    {
        Percent = percent;
        Price = price;
    }

    /// <summary>
    /// % скидки.
    /// </summary>
    public decimal Percent { get; }

    /// <summary>
    /// Сумма скидки.
    /// </summary>
    public decimal? Price { get; }
}