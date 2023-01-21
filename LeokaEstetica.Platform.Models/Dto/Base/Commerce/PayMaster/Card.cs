using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

/// <summary>
/// Класс модели данных карты пользователя.
/// </summary>
public class Card
{
    /// <summary>
    /// Номер карты.
    /// </summary>
    [JsonProperty("pan")]
    public string CardNumber { get; set; }

    /// <summary>
    /// Срок действия карты.
    /// </summary>
    [JsonProperty("expiry")]
    public string Expiry { get; set; }
    
    /// <summary>
    /// Cvc-код.
    /// </summary>
    [JsonProperty("cvc")]
    public string Cvc { get; set; }
}