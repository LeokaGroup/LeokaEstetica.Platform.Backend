namespace LeokaEstetica.Platform.Models.Dto.Output.Refunds;

/// <summary>
/// Класс выходной модели вычисления суммы возврата.
/// </summary>
public class CalculateRefundOutput
{
    /// <summary>
    /// Сумма к возврату.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }
}