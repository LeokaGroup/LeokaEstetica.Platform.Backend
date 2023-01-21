using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели создания заказа.
/// </summary>
public class CreateOrderOutput
{
    /// <summary>
    /// Id заказа.
    /// </summary>
    [JsonProperty("id")]
    public string OrderId { get; set; }

    /// <summary>
    /// Дата создания заказа.
    /// </summary>
    [JsonProperty("created")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Признак тестового платежа.
    /// </summary>
    [JsonProperty("testMode")]
    public bool IsTestMode { get; set; }
    
    /// <summary>
    /// Статус заказа.
    /// </summary>
    [JsonProperty("status")] 
    public string OrderStatus { get; set; }

    /// <summary>
    /// Код статуса заказа.
    /// </summary>
    [JsonProperty("resultCode")] 
    public string OrderCode { get; set; }
}