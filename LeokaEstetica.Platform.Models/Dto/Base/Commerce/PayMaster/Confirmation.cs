using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

/// <summary>
/// Класс модели способа подтверждения платежа.
/// </summary>
public class Confirmation
{
    /// <summary>
    /// Тип подтверждения.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Url.
    /// </summary>
    [JsonProperty("acsUrl")]
    public string AcsUrl { get; set; }

    /// <summary>
    /// Параметры.
    /// </summary>
    [JsonProperty("PAReq")]
    public string PaReq { get; set; }
}