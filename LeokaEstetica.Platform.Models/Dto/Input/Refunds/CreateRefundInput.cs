namespace LeokaEstetica.Platform.Models.Dto.Input.Refunds;

/// <summary>
/// Класс входной модели создания возврата.
/// </summary>
public class CreateRefundInput
{
    /// <summary>
    /// Id 
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Сумма возврата.
    /// </summary>
    public decimal Price { get; set; }
}