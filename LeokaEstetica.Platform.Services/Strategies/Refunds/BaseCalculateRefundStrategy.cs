using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Strategies.Refunds;

/// <summary>
/// Базовый класс семейства алгоритмов для вычисления суммы возврата.
/// </summary>
internal abstract class BaseCalculateRefundStrategy
{
    protected readonly ILogger<BaseCalculateRefundStrategy> Logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    protected BaseCalculateRefundStrategy(ILogger<BaseCalculateRefundStrategy> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Метод вычисляет сумму к возврату за вычетом суммы использованных дней.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Выходная модель возврата.</returns>
    protected abstract CalculateRefundOutput CalculateRefundAsync(long userId, long orderId);
}