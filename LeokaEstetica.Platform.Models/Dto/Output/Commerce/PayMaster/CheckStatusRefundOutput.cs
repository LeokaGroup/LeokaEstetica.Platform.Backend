using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели проверки статуса возврата.
/// </summary>
public class CheckStatusRefundOutput
{
    /// <summary>
    /// Системное название статуса заказа.
    /// </summary>
    [JsonProperty("status")]
    public string StatusSysName { get; set; }

    /// <summary>
    /// Id возврата в ПС.
    /// </summary>
    [JsonProperty("id")]
    public string RefundId { get; set; }
}