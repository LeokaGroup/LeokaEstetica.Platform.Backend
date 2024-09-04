namespace LeokaEstetica.Platform.Models.Dto.Base.Commerce;

/// <summary>
/// Базовый класс вычисления цены.
/// </summary>
public class BaseCalculatePrice
{
    /// <summary>
    /// Признак необходимости ожидания действий пользователя.
    /// </summary>
    public bool IsNeedUserAction { get; set; }
    
    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal? Price { get; set; }
}