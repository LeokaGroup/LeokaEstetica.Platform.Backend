using LeokaEstetica.Platform.Models.Dto.Base.Commerce;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;

/// <summary>
/// Класс входной модели создания заказа при запросе к ПС.
/// </summary>
public class CreateOrderYandexKassaRequest
{
    /// <summary>
    /// Id магазина.
    /// </summary>
    public int ShopId { get; set; }
    
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
    //public InvoicePayMaster Invoice { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    public Amount Amount { get; set; }

    /// <summary>
    /// Метод оплаты.
    /// </summary>
    public PaymentMethodData PaymentMethodData { get; set; }

    /// <summary>
    /// Описание платежа.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Способ подтверждения платежа.
    /// </summary>
    public Confirmation Confirmation { get; set; }

    /// <summary>
    /// Доп.данные, которые нужны нашей системе. ПС просто их вернет назад.
    /// </summary>
    public object Metadata { get; set; }
}