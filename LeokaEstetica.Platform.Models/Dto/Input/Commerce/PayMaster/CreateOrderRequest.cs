using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс входной модели создания заказа при запросе к ПС.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Ключ мерчанта (магазина).
    /// </summary>
    public Guid MerchantId { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public long FareRuleId { get; set; }

    /// <summary>
    /// Признак тестового режима.
    /// </summary>
    public bool TestMode { get; set; }
    
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
    /// Метод оплаты.
    /// </summary>
    [JsonProperty("paymentMethod")]
    public string PaymentMethod { get; set; }
}