using System.Text.Json.Serialization;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

namespace LeokaEstetica.Platform.Models.Dto.Output.Refunds;

/// <summary>
/// Класс выходной модели создания возврата.
/// </summary>
public class CreateRefundOutput
{
    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    [JsonPropertyName("id")]
    public string RefundOrderId { get; set; }

    /// <summary>
    /// Дата создания возврата.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Сумма возврата.
    /// </summary>
    public Amount Amount { get; set; }

    /// <summary>
    /// Статус возврата.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Id возврата.
    /// </summary>
    public long RefundId { get; set; }
}