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
    public string OrderName { get; set; }

    /// <summary>
    /// Описание заказа.
    /// </summary>
    public string OrderDetails { get; set; }

    /// <summary>
    /// Дата создания заказа.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Статус заказа.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Сумма заказа.
    /// </summary>
    public decimal Price { get; set; }
}