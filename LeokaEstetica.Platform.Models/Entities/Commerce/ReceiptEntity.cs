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
    /// Дата отправки чека пользователю.
    /// </summary>
    public DateTime? DateSend { get; set; }
}