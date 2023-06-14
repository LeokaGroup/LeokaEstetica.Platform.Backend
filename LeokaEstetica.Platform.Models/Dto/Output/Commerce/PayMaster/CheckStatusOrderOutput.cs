using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели проверки статуса платежа.
/// </summary>
public class CheckStatusOrderOutput
{
    /// <summary>
    /// Системное название статуса заказа.
    /// </summary>
    [JsonPropertyName("status")]
    public string StatusSysName { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    [JsonPropertyName("id")]
    public string PaymentId { get; set; }
}