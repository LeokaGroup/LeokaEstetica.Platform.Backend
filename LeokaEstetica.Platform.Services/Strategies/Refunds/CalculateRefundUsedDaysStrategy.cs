using System.Transactions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Strategies.Refunds;

/// <summary>
/// Класс реализует алгоритм вычисления суммы возврата на основании использованных дней.
/// ДС возвращаются только за неиспользованный период подписки.
/// </summary>
internal sealed class CalculateRefundUsedDaysStrategy : BaseCalculateRefundStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифов.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="userRepository">Репозиторий заказов.</param>
    public CalculateRefundUsedDaysStrategy(ILogger<BaseCalculateRefundStrategy> logger,
        ISubscriptionRepository subscriptionRepository,
        IFareRuleRepository fareRuleRepository,
        IUserRepository userRepository,
        IOrdersRepository ordersRepository)
        : base(logger,
            subscriptionRepository,
            fareRuleRepository,
            userRepository,
            ordersRepository)
    {
    }

    /// <summary>
    /// Метод вычисляет сумму к возврату за вычетом суммы использованных дней.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Выходная модель возврата.</returns>
    internal override async Task<CalculateRefundOutput> CalculateRefundAsync(long userId, long orderId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            // Получаем подписку.
            var currentSubscription = await SubscriptionRepository.GetUserSubscriptionAsync(userId);
        
            // Получаем тариф.
            var fareRule = await FareRuleRepository.GetByIdAsync(currentSubscription.ObjectId);

            // С бесплатным тарифом нет смысла работать. Возврат не делаем в таком кейсе.
            if (fareRule.IsFree)
            {
                Logger.LogWarning("Невозможно вычислить сумму возврата. Нельзя сделать возврат по бесплатному тарифу. " +
                                  $"UserId: {userId}. " +
                                  $"OrderId: {orderId}");
                return null;
            }

            // Вычисляем, сколько прошло дней использования подписки у пользователя.
            var usedDays = await UserRepository.GetUserSubscriptionUsedDateAsync(userId);

            // Если одна из дат пустая, то не можем вычислить сумму возврата. Возврат не делаем в таком кейсе.
            if (!usedDays.StartDate.HasValue || !usedDays.EndDate.HasValue)
            {
                Logger.LogWarning("Невозможно вычислить сумму возврата. Одна из дат подписки либо обе были null. " +
                                  $"UserId: {userId}. " +
                                  $"OrderId: {orderId}");
                return null;
            }

            // Вычисляем кол-во дней, за которые будем возвращать ДС.
            var referenceUsedDays = (int)Math.Round(usedDays.EndDate.Value.Subtract(usedDays.StartDate.Value)
                .TotalDays);

            // Получаем по какой цене был оформлен заказ.
            var orderPrice = (await OrdersRepository.GetOrderDetailsAsync(orderId, userId)).Price;

            // Вычисляем сумму к возврату.
            var resultRefundPrice = Math.Round(orderPrice / referenceUsedDays);

            // Не можем делать возвраты себе в ущерб.
            if (resultRefundPrice <= 0)
            {
                Logger.LogWarning($"Невозможно сделать возврат на сумму: {resultRefundPrice}" +
                                  $"UserId: {userId}. " +
                                  $"OrderId: {orderId}");
                return null;
            }
            
            scope.Complete();

            var result = new CalculateRefundOutput
            {
                OrderId = orderId,
                Price = resultRefundPrice
            };

            return result;
        }
        
        catch (Exception ex)
        {
            scope.Dispose();
            Logger.LogError(ex.Message, ex);
            throw;
        }
    }
}