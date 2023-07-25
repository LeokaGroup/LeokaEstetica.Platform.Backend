namespace LeokaEstetica.Platform.Models.Entities.Commerce;

/// <summary>
/// Класс сопоставляется с таблицей чеков Commerce.Receipts.
/// </summary>
public class ReceiptEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ReceiptId { get; set; }

    /// <summary>
    /// Id заказа, на основании которого формируется чек.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Дата создания чека.
    /// </summary>
    public DateTime DateCreated { get; set; }
    
    /// <summary>
    /// Статус возврата.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    public string ReceiptOrderId { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Тип чека в ПС.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}