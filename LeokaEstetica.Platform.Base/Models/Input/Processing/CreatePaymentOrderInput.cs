using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Base.Models.Input.Processing;

/// <summary>
/// Класс входной модели создания платежа заказа.
/// </summary>
public class CreatePaymentOrderInput
{
    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string? PaymentId { get; set; }

    /// <summary>
    /// Названия платежа.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Описание платежа.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Стоимость заказа.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Кол-во месяцев, на которые приобрели тариф.
    /// </summary>
    public short? PaymentMonth { get; set; }

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
    /// Тип валюты.
    /// </summary>
    public CurrencyTypeEnum CurrencyType { get; set; }

    /// <summary>
    /// Тип заказа.
    /// </summary>
    public OrderTypeEnum OrderType { get; set; }

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