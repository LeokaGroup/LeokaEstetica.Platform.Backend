using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;

/// <summary>
/// Класс входной модели создания заказа при запросе к ПС.
/// </summary>
public class CreateOrderYandexKassaRequest
{
    /// <summary>
    /// Признак тестового режима.
    /// </summary>
    [JsonProperty("test")]
    public bool TestMode { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    [JsonProperty("amount")]
    public Amount Amount { get; set; }

    /// <summary>
    /// Метод оплаты.
    /// </summary>
    [JsonProperty("payment_method_data")]
    public PaymentMethodData PaymentMethodData { get; set; }

    /// <summary>
    /// Описание платежа.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Способ подтверждения платежа.
    /// </summary>
    [JsonProperty("confirmation")]
    public Confirmation Confirmation { get; set; }

    /// <summary>
    /// Доп.данные, которые нужны нашей системе. ПС просто их вернет назад.
    /// </summary>
    [JsonProperty("metadata")]
    public object Metadata { get; set; }
}