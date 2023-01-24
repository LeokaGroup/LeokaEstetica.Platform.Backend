using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Processing.Models.Output;

/// <summary>
/// Класс выходной модели платежа.
/// </summary>
public class PaymentStatusOutput
{
    /// <summary>
    /// Статус платежа.
    /// </summary>
    [JsonProperty("status")]
    public string OrderStatus { get; set; }

    /// <summary>
    /// Дата создания платежа в ПС.
    /// </summary>
    [JsonProperty("created")]
    public string Created { get; set; }
}