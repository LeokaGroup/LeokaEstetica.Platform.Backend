using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;

/// <summary>
/// Класс описывает метод оплаты.
/// </summary>
public class PaymentMethodData
{
    /// <summary>
    /// Тип метода оплаты.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="type">Тип метода оплаты.</param>
    public PaymentMethodData(string type)
    {
        Type = type;
    }
}