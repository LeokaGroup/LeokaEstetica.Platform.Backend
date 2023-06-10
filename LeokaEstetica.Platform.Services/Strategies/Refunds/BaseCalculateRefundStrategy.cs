using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Strategies.Refunds;

/// <summary>
/// Базовый класс семейства алгоритмов для вычисления суммы возврата.
/// </summary>
public abstract class BaseCalculateRefundStrategy
{
    protected readonly ILogger<BaseCalculateRefundStrategy> Logger;
    protected readonly IUserRepository UserRepository;
    protected readonly IOrdersRepository OrdersRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="ordersRepository">Репозиторий заказов.</param>
    protected BaseCalculateRefundStrategy(ILogger<BaseCalculateRefundStrategy> logger, 
        IUserRepository userRepository, 
        IOrdersRepository ordersRepository)
    {
        Logger = logger;
        UserRepository = userRepository;
        OrdersRepository = ordersRepository;
    }

    /// <summary>
    /// Метод вычисляет сумму к возврату за вычетом суммы использованных дней.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Выходная модель возврата.</returns>
    internal abstract Task<CalculateRefundOutput> CalculateRefundAsync(long userId, long orderId);
}