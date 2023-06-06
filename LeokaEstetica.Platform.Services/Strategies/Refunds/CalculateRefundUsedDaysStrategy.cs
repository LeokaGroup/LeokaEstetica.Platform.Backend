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
    public CalculateRefundUsedDaysStrategy(ILogger<CalculateRefundUsedDaysStrategy> logger)
        : base(logger)
    {
    }

    /// <summary>
    /// Метод вычисляет сумму к возврату за вычетом суммы использованных дней.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Выходная модель возврата.</returns>
    protected override CalculateRefundOutput CalculateRefundAsync(long userId, long orderId)
    {
        throw new NotImplementedException();
    }
}