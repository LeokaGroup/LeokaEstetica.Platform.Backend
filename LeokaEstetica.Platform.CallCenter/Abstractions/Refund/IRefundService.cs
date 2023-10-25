using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.CallCenter.Abstractions.Refund;

/// <summary>
/// Абстракция сервиса возвратов в КЦ.
/// </summary>
public interface IRefundService
{
    /// <summary>
    /// Метод получает список возвратов для КЦ, которые не обработаны.
    /// </summary>
    /// <returns>Список необработанных возвратов.</returns>
    Task<IEnumerable<RefundEntity>> GetUnprocessedRefundsAsync();
}