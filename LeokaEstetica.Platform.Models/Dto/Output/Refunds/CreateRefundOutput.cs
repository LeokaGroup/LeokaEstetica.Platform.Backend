using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Refunds;

/// <summary>
/// Класс выходной модели создания возврата.
/// </summary>
public class CreateRefundOutput
{
    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    [JsonProperty("id")]
    public string RefundOrderId { get; set; }

    /// <summary>
    /// Дата создания возврата.
    /// </summary>
    [JsonProperty("created")]
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