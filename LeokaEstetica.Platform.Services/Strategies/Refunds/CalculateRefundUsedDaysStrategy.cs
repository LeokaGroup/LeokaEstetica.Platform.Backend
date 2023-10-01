using System.Transactions;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
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
    /// <param name="commerceService">Сервис заказов.</param>
    public CalculateRefundUsedDaysStrategy(ILogger<BaseCalculateRefundStrategy> logger, 
        ICommerceService commerceService) : base(logger, commerceService)
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
            var resultRefundPrice = await CommerceService.CalculatePriceSubscriptionFreeDaysAsync(userId, orderId);
            
            var result = new CalculateRefundOutput
            {
                OrderId = orderId,
                Price = resultRefundPrice
            };
            
            scope.Complete();

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