using LeokaEstetica.Platform.Models.Enums;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce;

/// <summary>
/// Класс описывает цену.
/// </summary>
public class Amount
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="currency"></param>
    public Amount(decimal value, string? currency)
    {
        Value = value;
        Currency = currency;
    }

    /// <summary>
    /// Сумма.
    /// </summary>
    [JsonProperty("value")]
    public decimal Value { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }
}