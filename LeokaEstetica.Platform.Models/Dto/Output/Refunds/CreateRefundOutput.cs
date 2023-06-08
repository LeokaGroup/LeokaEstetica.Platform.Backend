using System.Text.Json.Serialization;

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
    public string RefundId { get; set; }

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
    [JsonPropertyName("value")]
    public decimal Price { get; set; }

    /// <summary>
    /// Код валюты.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Статус возврата.
    /// </summary>
    public string Status { get; set; }
}