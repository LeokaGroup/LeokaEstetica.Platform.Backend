using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;

/// <summary>
/// Базовый класс выходной модели создания платежа.
/// </summary>
public interface ICreateOrderOutput
{
    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    string? PaymentId { get; set; }

    /// <summary>
    /// Статус платежа в ПС.
    /// </summary>
    string? OrderStatus { get; set; }
    
    /// <summary>
    /// Подтверждение платежа.
    /// </summary>
    [JsonProperty("confirmation")]
    public ConfirmationOutput? Confirmation { get; set; }
}