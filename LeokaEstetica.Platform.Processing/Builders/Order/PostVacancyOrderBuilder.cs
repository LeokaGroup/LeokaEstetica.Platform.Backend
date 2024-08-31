using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Processing.BuilderData;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Класс билдера заказа платной публикации вакансии.
/// </summary>
internal class PostVacancyOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Входная модель заказа на платное размещение вакансии.
    /// </summary>
    internal VacancyInput? VacancyOrderData { get; set; }

    /// <summary>
    /// Ивент для кролика.
    /// </summary>
    internal PostVacancyOrderEvent? OrderEvent { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    public PostVacancyOrderBuilder(ISubscriptionRepository subscriptionRepository,
        ICommerceRepository commerceRepository)
        : base(subscriptionRepository, commerceRepository)
    {
        VacancyOrderData ??= new VacancyInput();
        OrderEvent ??= new PostVacancyOrderEvent();
        OrderData ??= new OrderData();
    }

    /// <inheritdoc />
    protected internal override Task FillMonthAsync()
    {
        throw new NotImplementedException("Не предполагается для билдера вакансий.");
    }

    /// <inheritdoc />
    protected internal override Task FillFareRuleNameAsync()
    {
        OrderData!.FareRuleName = "Публикация вакансии.";

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected internal override async Task CalculateFareRulePriceAsync()
    {
        var userSubscription = await SubscriptionRepository.GetUserSubscriptionByUserIdAsync(OrderData!.CreatedBy);

        if (userSubscription is null)
        {
            throw new InvalidOperationException("Ошибка получения подписки пользователя. " +
                                                $"UserId: {OrderData!.CreatedBy}.");
        }

        // Получаем услугу по тарифу.
        var fees = await CommerceRepository.GetFeesByFareRuleIdAsync(userSubscription.RuleId);

        if (fees is null)
        {
            throw new InvalidOperationException("Ошибка получения услуги. " +
                                                $"UserId: {OrderData!.CreatedBy}. " +
                                                $"RuleId: {userSubscription.RuleId}.");
        }

        // Если цена 0, значит бесплатно.
        // Бесплатный заказ конечно не будем проводить через ПС.
        OrderData.Amount = new Amount(fees.FeesPrice ?? 0, CurrencyTypeEnum.RUB.ToString());

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    protected internal override async Task<OrderEvent> CreateOrderEventAsync()
    {
        OrderEvent = new PostVacancyOrderEvent
        {
            CreatedBy = OrderData!.CreatedBy > 0
                ? OrderData!.CreatedBy
                : throw new InvalidOperationException(
                    "CreatedBy неизвестен. Пользователь на этом этапе уже должен быть известен. " +
                    $"OrderData: {JsonConvert.SerializeObject(OrderData)}."),
            Currency = CurrencyTypeEnum.RUB,
            Month = null,
            OrderId = OrderData.OrderId ?? throw new InvalidOperationException(
                "OrderId заказа вакансии не был известен. " +
                "Он обязательно должен был быть уже известен, так как на этом этапе заказ уже создан в нашей БД. " +
                $"OrderData: {JsonConvert.SerializeObject(OrderData)}."),
            OrderType = OrderTypeEnum.CreateVacancy,
            PaymentId = OrderData.PaymentId ?? throw new InvalidOperationException(
                "PaymentId заказа вакансии не был известен. " +
                "Он обязательно должен был быть уже известен, так как на этом этапе заказ уже создан в ПС. " +
                $"OrderData: {JsonConvert.SerializeObject(OrderData)}."),
            Price = (OrderData.Amount
                     ?? throw new InvalidOperationException(
                         "Amount не был заполнен. " +
                         $"OrderData: {JsonConvert.SerializeObject(OrderData)}.")).Value >= 0
                ? OrderData.Amount.Value
                : throw new InvalidOperationException(
                    "Цена заказа вакансии не была известна и не была равна нулю. " +
                    "Она обязательно должна была быть уже известна, так как на этом этапе заказ" +
                    " уже зафиксировал цену. " +
                    $"OrderData: {JsonConvert.SerializeObject(OrderData)}."),
            PublicId = OrderData.PublicId != Guid.Empty
                ? OrderData.PublicId
                : throw new InvalidOperationException("PublicId невалиден. " +
                                                      $"OrderData: {JsonConvert.SerializeObject(OrderData)}.")
        };

        return await Task.FromResult(OrderEvent);
    }
}