namespace LeokaEstetica.Platform.Models.Dto.Common.Cache.Output;

/// <summary>
/// Класс выходной модели создания заказа в кэше.
/// </summary>
public class CreateOrderCacheOutput
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
    public short Month { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string? FareRuleName { get; set; }
}