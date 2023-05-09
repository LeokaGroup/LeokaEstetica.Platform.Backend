namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели создания заказа в кэше.
/// </summary>
public class CreateOrderCacheOutput
{
    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Стоимость тарифа (с учетом всех скидок, сервисов, акций).
    /// </summary>
    public decimal Price { get; set; }
}