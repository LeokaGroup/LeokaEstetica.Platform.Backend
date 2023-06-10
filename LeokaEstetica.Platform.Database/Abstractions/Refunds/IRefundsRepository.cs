using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Abstractions.Refunds;

/// <summary>
/// Абстракция репозитория возвратов.
/// </summary>
public interface IRefundsRepository
{
    /// <summary>
    /// Метод сохраняет возврат.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="dateCreated">Дата создания возврата в ПС.</param>
    /// <param name="status">Статус возврата в ПС.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <returns>Данные возврата.</returns>
    Task<RefundEntity> SaveRefundAsync(string paymentId, decimal price, DateTime dateCreated, string status,
        string refundOrderId);
}