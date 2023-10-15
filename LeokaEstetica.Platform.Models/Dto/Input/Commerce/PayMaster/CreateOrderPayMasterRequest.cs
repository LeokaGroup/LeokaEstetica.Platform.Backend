using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс входной модели создания заказа при запросе к ПС.
/// </summary>
public class CreateOrderPayMasterRequest
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
    public InvoicePayMaster Invoice { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    public AmountPayMaster Amount { get; set; }

    /// <summary>
    /// Метод оплаты.
    /// </summary>
    public string PaymentMethod { get; set; }
}