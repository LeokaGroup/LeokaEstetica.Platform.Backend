using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Refunds;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Repositories.Refunds;

/// <summary>
/// Класс реализует методы репозитория возвратов.
/// </summary>
public sealed class RefundsRepository : IRefundsRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public RefundsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод сохраняет возврат.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="dateCreated">Дата создания возврата в ПС.</param>
    /// <param name="status">Статус возврата в ПС.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <returns>Данные возврата.</returns>
    public async Task<RefundEntity> SaveRefundAsync(string paymentId, decimal price, DateTime dateCreated,
        string status, string refundOrderId)
    {
        var result = new RefundEntity
        {
            PaymentId = paymentId,
            Price = price
        };

        await _pgContext.Refunds.AddAsync(result);
        await _pgContext.SaveChangesAsync();

        return result;
    }
}