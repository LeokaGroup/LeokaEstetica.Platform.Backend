using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Processing.BuilderData;

/// <summary>
/// Класс для данных, необходимых для строителей. Зависимости и тд.
/// </summary>
internal class OrderData
{
    /// <summary>
    /// Атрибуты тарифа.
    /// </summary>
    public FareRuleAttributeValueOutput? FareRuleAttributeValues { get; set; }

    /// <summary>
    /// Название тарифа, может быть не заполненно.
    /// Его нету у доп.заказах, которые можно оформлять отдельно.
    /// Оплата размещения вакансии и тд, не основной тариф.
    /// </summary>
    public string? FareRuleName { get; set; }

    /// <summary>
    /// Id пользователя, который оформляет заказ.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public long? RuleId { get; set; }

    /// <summary>
    /// Тип заказа.
    /// </summary>
    public OrderTypeEnum OrderType { get; set; }

    /// <summary>
    /// Кол-во сотрудников.
    /// </summary>
    public short? EmployeesCount { get; set; }

    /// <summary>
    /// Кол-во месяцев подписки. Может не быть задан для доп.заказов.
    /// </summary>
    public short? Month { get; set; }
}