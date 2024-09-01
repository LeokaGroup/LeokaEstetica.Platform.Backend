using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Enums;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Orders;

/// <summary>
/// Класс выходной модели создания заказа в кэше или в очереди кролика.
/// </summary>
public class CreateOrderOutput : ICreateOrderOutput, IFrontError
{
    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    public short? Month { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string? FareRuleName { get; set; }

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
    public DateTime CreatedAt { get; set; }

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

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string? PaymentId { get; set; }

    /// <summary>
    /// Статус платежа в ПС.
    /// </summary>
    public string? OrderStatus { get; set; }
    
    /// <summary>
    /// Подтверждение платежа.
    /// </summary>
    [JsonProperty("confirmation")]
    public ConfirmationOutput? Confirmation { get; set; }
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure>? Errors { get; set; }
}