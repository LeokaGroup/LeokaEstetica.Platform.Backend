namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели услуг и сервисов пользователя.
/// </summary>
public class ServiceCacheOutput
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
    /// Название тарифа.
    /// </summary>
    public string FareRuleName { get; set; }

    /// <summary>
    /// Название услуги.
    /// </summary>
    public string ServiceName { get; set; }
}