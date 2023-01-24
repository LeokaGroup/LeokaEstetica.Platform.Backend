using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

/// <summary>
/// Класс описывает цену.
/// </summary>
public class Amount
{
    /// <summary>
    /// Значение стоимости.
    /// </summary>
    [JsonProperty("value")]
    public decimal Value { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }
}