using LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;

/// <summary>
/// Класс выходной модели подтверждения платежа.
/// </summary>
public class ConfirmationOutput : Confirmation
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="type">Способ потдверждения платежа.</param>
    /// <param name="returnUrl">Url для коллбека после оплаты пользователя.</param>
    public ConfirmationOutput(string type, string returnUrl) : base(type, returnUrl)
    {
    }

    /// <summary>
    /// Url на оплату для пользователя.
    /// </summary>
    [JsonProperty("confirmation_url")]
    public string ConfirmationUrl { get; set; }
}