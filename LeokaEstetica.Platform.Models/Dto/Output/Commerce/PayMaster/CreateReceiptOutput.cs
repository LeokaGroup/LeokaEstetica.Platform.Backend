namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели создания чека.
/// </summary>
public class CreateReceiptOutput
{
    /// <summary>
    /// Id чека в ПС.
    /// </summary>
    public string ReceiptId { get; set; }

    /// <summary>
    /// Дата создания чека в ПС.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Тип чека.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Статус чека в ПС.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Id возврата.
    /// </summary>
    public long RefundId { get; set; }
}