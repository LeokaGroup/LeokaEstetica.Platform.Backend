namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели проверки остатка суммы с прошлой подписки.
/// </summary>
public class OrderFreeOutput
{
    /// <summary>
    /// Остаток суммы с прошлой подписки.
    /// </summary>
    public decimal FreePrice { get; set; }

    /// <summary>
    /// Цена тарифа.
    /// </summary>
    public decimal Price { get; set; }
}