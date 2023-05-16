namespace LeokaEstetica.Platform.Models.Entities.Commerce;

/// <summary>
/// Класс сопоставляется с таблицей сервисов к тарифам Commerce.Products.
/// </summary>
public class ProductEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Тип сервиса тарифов.
    /// </summary>
    public string ProductType { get; set; }

    /// <summary>
    /// Название сервиса тарифов.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Дает ли сервис скидку тарифу.
    /// </summary>
    public bool IsDiscount { get; set; }

    /// <summary>
    /// % скидки (1-100%).
    /// </summary>
    public int PercentDiscount { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Цена сервиса (ее может и не быть, это будет означать, что сервис дает скидку).
    /// Если цены сервиса нет, то значит признак скидки стоит true.
    /// </summary>
    public decimal? ProductPrice { get; set; }
}