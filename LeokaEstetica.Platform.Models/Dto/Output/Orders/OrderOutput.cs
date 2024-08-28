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
}