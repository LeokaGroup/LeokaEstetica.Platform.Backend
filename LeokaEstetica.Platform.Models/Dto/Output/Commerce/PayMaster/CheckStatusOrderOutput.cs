using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели проверки статуса платежа.
/// </summary>
public class CheckStatusOrderOutput
{
    /// <summary>
    /// Системное название статуса заказа.
    /// </summary>
    [JsonProperty("status")]
    public string StatusSysName { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    [JsonProperty("id")]
    public string PaymentId { get; set; }
}