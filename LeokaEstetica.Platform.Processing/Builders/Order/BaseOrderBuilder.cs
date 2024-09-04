using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Processing.BuilderData;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Processing")]

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Базовый абстрактный класс билдера заказов.
/// </summary>
public abstract class BaseOrderBuilder
{
    protected readonly ISubscriptionRepository SubscriptionRepository;
    protected readonly ICommerceRepository CommerceRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// </summary>
    protected internal BaseOrderBuilder(ISubscriptionRepository subscriptionRepository,
        ICommerceRepository commerceRepository)
    {
        SubscriptionRepository = subscriptionRepository;
        CommerceRepository = commerceRepository;
        OrderData ??= new OrderData();
    }

    /// <summary>
    /// Класс с данными, которые могут использоваться в билдерах заказов.
    /// </summary>
    internal OrderData? OrderData { get; set; }

    /// <summary>
    /// Метод заполняет кол-во месяцев подписки, если срок предполагается.
    /// </summary>
    protected internal abstract Task FillMonthAsync();

    /// <summary>
    /// Метод заполняет название тарифа.
    /// </summary>
    protected internal abstract Task FillFareRuleNameAsync();

    /// <summary>
    /// Метод вычисляет сумму заказа.
    /// </summary>
    protected internal abstract Task CalculateFareRulePriceAsync();

    /// <summary>
    /// Метод подготавливает ивент для кролика.
    /// </summary>
    /// <returns>Заполненный ивент для кролика.</returns>
    protected internal abstract Task<OrderEvent> CreateOrderEventAsync();

    /// <summary>
    /// Метод заполняет тип заказа.
    /// </summary>
    protected internal abstract Task FillOrderTypeAsync();
}