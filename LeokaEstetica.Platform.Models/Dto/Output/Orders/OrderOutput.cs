using LeokaEstetica.Platform.Models.Enums;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Models.Dto.Output.Orders;

/// <summary>
/// Класс выходной модели заказа.
/// </summary>
public class OrderOutput
{
    /// <summary>
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Название заказа.
    /// </summary>
    public string? OrderName { get; set; }

    /// <summary>
    /// Описание заказа.
    /// </summary>
    public string? OrderDetails { get; set; }

    /// <summary>
    /// Дата создания заказа.
    /// </summary>
    public string? DateCreated { get; set; }

    /// <summary>
    /// Статус заказа.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Сумма заказа.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Значение енамки типа валюты.
    /// </summary>
    public CurrencyTypeEnum CurrencyValue { get; set; }

    /// <summary>
    /// Тип валюты.
    /// </summary>
    public IEnum CurrencyType
    {
        get => new Enum(CurrencyValue);
        set => CurrencyValue = Enum.FromString<CurrencyTypeEnum>(value.Value);
    }

    /// <summary>
    /// Системное название статуса заказа.
    /// </summary>
    public string? StatusSysName { get; set; }
    
    /// <summary>
    /// Тип заказа.
    /// </summary>
    public OrderTypeEnum OrderType { get; set; }
    
    /// <summary>
    /// Системное название статуса платежа в ПС.
    /// </summary>
    public string? PaymentStatusSysName { get; set; }

    /// <summary>
    /// Название статуса платежа в ПС.
    /// </summary>
    public string? PaymentStatusName { get; set; }

    /// <summary>
    /// Дата создания платежа в ПС.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Ставка НДС в %.
    /// </summary>
    public int? VatRate { get; set; }

    /// <summary>
    /// Цена с НДС (цена без НДС + НДС).
    /// </summary>
    public decimal? PriceVat { get; set; }

    /// <summary>
    /// % скидки.
    /// </summary>
    public int? Discount { get; set; }

    /// <summary>
    /// Цена со скидкой.
    /// </summary>
    public decimal? DiscountPrice { get; set; }

    /// <summary>
    /// Общая сумма заказа (вместе со скидками и НДС - если они были).
    /// </summary>
    public decimal TotalPrice { get; set; }
}