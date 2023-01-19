namespace LeokaEstetica.Platform.Models.Entities.Commerce;

/// <summary>
/// Класс сопоставляется с таблицей заказов Commerce.Orders.
/// </summary>
public class OrderEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Название заказа.
    /// </summary>
    public string OrderName { get; set; }

    /// <summary>
    /// Детальное описание заказа.
    /// </summary>
    public string OrderDetails { get; set; }

    /// <summary>
    /// Дата создания заказа.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id пользователя, оформившего заказ.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название статуса заказа. По дефолту "Новый".
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Цена заказа.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Кол-во месяцев тарифа, который оплачены.
    /// </summary>
    public short PaymentMonth { get; set; }

    /// <summary>
    /// Валюта заказа.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Системное название статуса заказа. По дефолту "New".
    /// </summary>
    public string StatusSysName { get; set; }
}