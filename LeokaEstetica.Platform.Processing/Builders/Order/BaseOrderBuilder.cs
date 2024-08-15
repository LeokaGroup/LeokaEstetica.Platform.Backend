using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Processing.BuilderData;

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Базовый абстрактный класс билдера заказов.
/// </summary>
internal abstract class BaseOrderBuilder
{
    public CreateOrderCache? OrderCache { get; set; }
    
    /// <summary>
    /// Класс с данными, которые могут использоваться в билдерах заказов.
    /// </summary>
    public OrderData? OrderData { get; set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    protected BaseOrderBuilder()
    {
        OrderCache = new CreateOrderCache();
    }

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