namespace LeokaEstetica.Platform.Models.Dto.Output.Refunds;

/// <summary>
/// Класс выходной модели возврата.
/// </summary>
public class RefundOutput
{
    /// <summary>
    /// Id возврата.
    /// </summary>
    public string RefundId { get; set; }

    /// <summary>
    /// Дата создания возврата.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Сумма возврата.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Статус возврата.
    /// </summary>
    public string Status { get; set; }
}