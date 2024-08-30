using LeokaEstetica.Platform.Processing.BuilderData;

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Базовый абстрактный класс билдера заказов.
/// </summary>
internal abstract class BaseOrderBuilder
{
    /// <summary>
    /// Класс с данными, которые могут использоваться в билдерах заказов.
    /// </summary>
    public OrderData? OrderData { get; set; }

    /// <summary>
    /// Метод заполняет кол-во месяцев подписки, если срок предполагается.
    /// </summary>
    public abstract Task FillMonthAsync();

    /// <summary>
    /// Метод заполняет название тарифа.
    /// </summary>
    public abstract Task FillFareRuleNameAsync();

    /// <summary>
    /// Метод вычисляет сумму заказа.
    /// </summary>
    public abstract Task CalculateFareRulePriceAsync();
}