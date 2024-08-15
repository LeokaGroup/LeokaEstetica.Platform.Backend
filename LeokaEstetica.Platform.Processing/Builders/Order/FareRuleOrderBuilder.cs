using LeokaEstetica.Platform.Models.Dto.Common.Cache;

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Класс билдера заказа тарифа.
/// </summary>
internal class FareRuleOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="orderCacheInput">Модель заказа для заполнения ее полей.</param>
    public FareRuleOrderBuilder()
    {
        OrderCache = new CreateOrderCache();
    }

    /// <inheritdoc />
    public override Task FillMonthAsync()
    {
        OrderCache!.Month = OrderCache.Month;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task FillFareRuleNameAsync()
    {
        OrderCache!.FareRuleName = "";
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task CalculateFareRulePriceAsync()
    {
        // Цена тарифа = минимальная цена тарифа * кол-во сотрудников * кол-во месяцев.
        OrderCache!.Price = OrderData!.FareRuleAttributeValues!.MinValue!.Value * OrderData.EmployeesCount!.Value *
                            OrderData.Month!.Value;

        return Task.CompletedTask;
    }
}