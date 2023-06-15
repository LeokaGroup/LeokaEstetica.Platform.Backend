using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Strategies.Refunds;

/// <summary>
/// Базовый класс семейства алгоритмов для вычисления суммы возврата.
/// </summary>
public abstract class BaseCalculateRefundStrategy
{
    protected readonly ILogger<BaseCalculateRefundStrategy> Logger;
    protected readonly ICommerceService CommerceService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="commerceService">Сервис заказов.</param>
    protected BaseCalculateRefundStrategy(ILogger<BaseCalculateRefundStrategy> logger, 
        ICommerceService commerceService)
    {
        Logger = logger;
        CommerceService = commerceService;
    }

    /// <summary>
    /// Метод вычисляет сумму к возврату за вычетом суммы использованных дней.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Выходная модель возврата.</returns>
    internal abstract Task<CalculateRefundOutput> CalculateRefundAsync(long userId, long orderId);
}