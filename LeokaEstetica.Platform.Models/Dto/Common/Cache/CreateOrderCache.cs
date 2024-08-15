using LeokaEstetica.Platform.Models.Enums;
using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache;

/// <summary>
/// Класс модели для хранения заказа в кэше.
/// </summary>
[ProtoContract]
public class CreateOrderCache
{
    /// <summary>
    /// Id тарифа.
    /// </summary>
    [ProtoMember(1)]
    public int RuleId { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    [ProtoMember(2)]
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    [ProtoMember(3)]
    public short? Month { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    [ProtoMember(4)]
    public long CreatedBy { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    [ProtoMember(5)]
    public string? FareRuleName { get; set; }

    /// <summary>
    /// Тип заказа.
    /// </summary>
    [ProtoMember(6)]
    public OrderTypeEnum OrderType { get; set; }
    
    /// <summary>
    /// Системное название статуса платежа в ПС.
    /// </summary>
    [ProtoMember(7)]
    public string? PaymentStatusSysName { get; set; }

    /// <summary>
    /// Название статуса платежа в ПС.
    /// </summary>
    [ProtoMember(8)]
    public string? PaymentStatusName { get; set; }

    /// <summary>
    /// Дата создания платежа в ПС.
    /// </summary>
    [ProtoMember(9)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    [ProtoMember(10)]
    public string? Currency { get; set; }

    /// <summary>
    /// Ставка НДС в %.
    /// </summary>
    [ProtoMember(11)]
    public int? VatRate { get; set; }

    /// <summary>
    /// Цена с НДС (цена без НДС + НДС).
    /// </summary>
    [ProtoMember(12)]
    public decimal? PriceVat { get; set; }

    /// <summary>
    /// % скидки.
    /// </summary>
    [ProtoMember(13)]
    public int? Discount { get; set; }

    /// <summary>
    /// Цена со скидкой.
    /// </summary>
    [ProtoMember(14)]
    public decimal? DiscountPrice { get; set; }

    /// <summary>
    /// Общая сумма заказа (вместе со скидками и НДС - если они были).
    /// </summary>
    [ProtoMember(15)]
    public decimal TotalPrice { get; set; }
}