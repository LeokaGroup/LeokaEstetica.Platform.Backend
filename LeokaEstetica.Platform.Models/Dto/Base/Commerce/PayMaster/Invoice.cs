using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

/// <summary>
/// Класс описывает детали счета.
/// </summary>
public class Invoice
{
    /// <summary>
    /// Описание платежа.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }
}