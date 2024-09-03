using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
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
    internal FareRuleAttributeValueOutput? FareRuleAttributeValues { get; set; }

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
    public int? EmployeesCount { get; set; }

    /// <summary>
    /// Кол-во месяцев подписки. Может не быть задан для доп.заказов.
    /// </summary>
    public short? Month { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string? PaymentId { get; set; }

    /// <summary>
    /// Данные о цене заказа.
    /// </summary>
    public Amount? Amount { get; set; }

    /// <summary>
    /// Пользователь, который оформляет заказ.
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Id заказа.
    /// При оформлении тарифа его еще нету.
    /// При оформлении вакансии, он уже есть.
    /// </summary>
    public long? OrderId { get; set; }

    /// <summary>
    /// Название тарифа из кэша. Пока лишь для оплаты тарифа.
    /// </summary>
    public string? fareRuleNameFromCache { get; set; }
}