using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;

/// <summary>
/// Класс описывает способ подтверждения платежа.
/// </summary>
public class Confirmation
{
    /// <summary>
    /// Тип способа подтверждения платежа.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Url коллбека.
    /// </summary>
    [JsonProperty("return_url")]
    public string ReturnUrl { get; set; }
    
    /// <summary>
    /// Конструктор.
    /// <param name="type">Тип способа подтверждения платежа.</param>
    /// <param name="returnUrl">Url коллбека.</param>
    /// </summary>
    public Confirmation(string type, string returnUrl)
    {
        Type = type;
        ReturnUrl = returnUrl;
    }
}