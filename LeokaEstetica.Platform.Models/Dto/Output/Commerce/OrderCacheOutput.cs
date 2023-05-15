namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели заказа в кэше.
/// </summary>
public class OrderCacheOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Процент скидки.
    /// </summary>
    public decimal Percent { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Id пользователя, которому принадлежит заказ.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Список сервисов услуг.
    /// </summary>
    public List<string> Products { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string FareRuleName { get; set; }
}