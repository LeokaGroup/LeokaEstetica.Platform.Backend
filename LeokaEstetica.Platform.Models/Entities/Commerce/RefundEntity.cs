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

    /// <summary>
    /// Дата создания возврата.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Статус возврата.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    public string RefundOrderId { get; set; }

    /// <summary>
    /// Признак ручного создания возврата.
    /// </summary>
    public bool IsManual { get; set; }
}