namespace LeokaEstetica.Platform.Models.Entities.Commerce;

/// <summary>
/// Класс сопоставляется с таблицей возвратов Commerce.Refunds.
/// </summary>
public class RefundEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long RefundId { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Сумма возврата.
    /// </summary>
    public decimal Price { get; set; }
}