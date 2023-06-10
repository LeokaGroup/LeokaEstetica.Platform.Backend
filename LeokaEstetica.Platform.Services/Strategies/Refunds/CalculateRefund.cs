using LeokaEstetica.Platform.Models.Dto.Output.Refunds;

namespace LeokaEstetica.Platform.Services.Strategies.Refunds;

/// <summary>
/// Класс представляет семейство алгоритмов для вычисления суммы возврата.
/// </summary>
internal sealed class CalculateRefund
{
    /// <summary>
    /// Метод вычисляет сумму к возврату за вычетом суммы использованных дней.
    /// </summary>
    /// <param name="strategy">Стратегия поиска.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Выходная модель возврата.</returns>
    internal async Task<CalculateRefundOutput> CalculateRefundAsync(BaseCalculateRefundStrategy strategy, long userId,
        long orderId)
    {
        if (strategy is not null)
        {
            return await strategy.CalculateRefundAsync(userId, orderId);
        }

        return null;
    }
}