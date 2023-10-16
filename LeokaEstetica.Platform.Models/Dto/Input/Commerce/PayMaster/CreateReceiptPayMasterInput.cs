using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс входной модели создания чека.
/// </summary>
[Serializable]
public class CreateReceiptPayMasterInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="amount">Данные суммы.</param>
    /// <param name="type">Тип чека.</param>
    /// <param name="client">Данные пользователя.</param>
    /// <param name="items">Позиции чека.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="refundId">Id возврата.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    public CreateReceiptPayMasterInput(string paymentId, Amount amount, string type, ClientInput client,
        List<ReceiptItem> items, long orderId, long refundId, string refundOrderId)
    {
        PaymentId = paymentId;
        Amount = amount;
        Type = type;
        Client = client;
        Items = items;
        OrderId = orderId;
        RefundId = refundId;
        RefundOrderId = refundOrderId;
    }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Сумма чека.
    /// </summary>
    public Amount Amount { get; set; }

    /// <summary>
    /// Тип чека. <see cref="ReceiptTypeEnum"/>
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Данные пользователя в ПС.
    /// </summary>
    public ClientInput Client { get; set; }

    /// <summary>
    /// Позиции чека.
    /// </summary>
    public List<ReceiptItem> Items { get; set; }

    /// <summary>
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }
    
    /// <summary>
    /// Id возврата.
    /// </summary>
    public long RefundId { get; set; }

    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    public string RefundOrderId { get; set; }
}