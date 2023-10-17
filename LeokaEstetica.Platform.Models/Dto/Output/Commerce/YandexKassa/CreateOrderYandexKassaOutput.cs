using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;

/// <summary>
/// Класс выходной модели создания заказа в ЮKassa.
/// </summary>
public class CreateOrderYandexKassaOutput : ICreateOrderOutput, IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    [JsonProperty("id")]
    public string PaymentId { get; set; }
    
    /// <summary>
    /// Статус платежа в ПС.
    /// </summary>
    [JsonProperty("status")]
    public string OrderStatus { get; set; }

    /// <summary>
    /// Подтверждение платежа.
    /// </summary>
    [JsonProperty("confirmation")]
    public ConfirmationOutput Confirmation { get; set; }

    /// <summary>
    /// Дата создания заказа в ПС.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime Created { get; set; }
}