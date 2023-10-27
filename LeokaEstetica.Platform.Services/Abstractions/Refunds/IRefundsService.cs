using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Services.Abstractions.Refunds;

/// <summary>
/// Абстракция сервиса возвратов в нашей системе.
/// </summary>
public interface IRefundsService
{
    /// <summary>
    /// Метод вычисляет сумму возврата заказа.
    /// Возврат делается только за неиспользованный период подписки.
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// </summary>
    /// <returns>Выходная модель.</returns>
    Task<CalculateRefundOutput> CalculateRefundAsync(string account, string token);

    /// <summary>
    /// Метод создает возврат по заказу.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Выходная модель.</returns>
    Task<CreateRefundOutput> CreateRefundAsync(long orderId, decimal price, string account, string token);
    
    /// <summary>
    /// Метод получает список возвратов для КЦ, которые не обработаны.
    /// </summary>
    /// <returns>Список необработанных возвратов.</returns>
    Task<IEnumerable<RefundEntity>> GetUnprocessedRefundsAsync();
}