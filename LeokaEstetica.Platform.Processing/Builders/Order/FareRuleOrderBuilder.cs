using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Processing.BuilderData;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Processing")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Controllers")]
[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Класс билдера заказа тарифа.
/// </summary>
internal class FareRuleOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Данные заказа для кэша.
    /// </summary>
    public CreateOrderCache OrderCache { get; set; }

    /// <summary>
    /// Подготовительные данные.
    /// </summary>
    public OrderData OrderData { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    public FareRuleOrderBuilder(ISubscriptionRepository subscriptionRepository,
        ICommerceRepository commerceRepository)
        : base(subscriptionRepository, commerceRepository)
    {
        OrderCache ??= new CreateOrderCache();
        OrderData ??= new OrderData();
    }

    /// <inheritdoc />
    protected internal override Task FillMonthAsync()
    {
        OrderCache.Month = OrderData.Month;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected internal override Task FillFareRuleNameAsync()
    {
        // OrderCache.FareRuleName = "Оплата тарифа: " + OrderData!.fareRuleNameFromCache
        //                                             + $" (на {OrderCache.Month} мес.)";
        OrderCache.FareRuleName = OrderData.FareRuleName;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected internal override Task CalculateFareRulePriceAsync()
    {
        // Цена тарифа = минимальная цена тарифа * кол-во сотрудников * кол-во месяцев.
        OrderCache.Price = OrderData!.FareRuleAttributeValues!.MinValue!.Value * OrderData.EmployeesCount!.Value *
                            OrderData.Month!.Value;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected internal override Task<OrderEvent> CreateOrderEventAsync()
    {
        throw new NotImplementedException("При оформлении заказа на тариф, кролик не задействуется в билдере.");
    }

    /// <inheritdoc />
    protected internal override Task FillOrderTypeAsync()
    {
        OrderCache.OrderType = OrderData.OrderType;
        
        return Task.CompletedTask;
    }
}