using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс входной модели создания заказа.
/// </summary>
public class CreateOrderInput
{
    /// <summary>
    /// Ключ мерчанта (магазина).
    /// </summary>
    [JsonProperty("merchantId")]
    public Guid MerchantId { get; set; }

    /// <summary>
    /// Детали счета.
    /// </summary>
    [JsonProperty("invoice")]
    public Invoice Invoice { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    [JsonProperty("amount")]
    public Amount Amount { get; set; }
    
    /// <summary>
    /// Данные платежа.
    /// </summary>
    [JsonProperty("paymentData")]
    public PaymentData PaymentData { get; set; }

    /// <summary>
    /// Способ подтверждения платежа.
    /// </summary>
    [JsonProperty("confirmation")]
    public Confirmation Confirmation { get; set; }
}