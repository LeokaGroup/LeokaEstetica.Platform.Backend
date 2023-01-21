using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

/// <summary>
/// Класс модели с данными карты пользователя.
/// </summary>
public class PaymentData
{
    /// <summary>
    /// Метод оплаты.
    /// </summary>
    [JsonProperty("paymentMethod")]
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Данные карты.
    /// </summary>
    [JsonProperty("card")]
    public Card Card { get; set; }
}