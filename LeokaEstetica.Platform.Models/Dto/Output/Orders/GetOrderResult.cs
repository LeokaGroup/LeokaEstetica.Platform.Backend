namespace LeokaEstetica.Platform.Models.Dto.Output.Orders;

/// <summary>
/// Класс результата списка заказов.
/// </summary>
public class GetOrderResult
{
    /// <summary>
    /// Список заказов.
    /// </summary>
    public IEnumerable<OrderOutput> Orders { get; set; }

    /// <summary>
    /// Кол-во заказов.
    /// </summary>
    public int Total => Orders.Count();
}