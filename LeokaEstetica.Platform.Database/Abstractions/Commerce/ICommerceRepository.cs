using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Abstractions.Commerce;

/// <summary>
/// TODO: Отрефачить разбив логику заказов в отдельный репозиторий OrderRepository.
/// Абстракция репозитория коммерции.
/// </summary>
public interface ICommerceRepository
{
    /// <summary>
    /// Метод создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderInput">Входная модель.</param>
    /// <returns>Данные заказа.</returns>
    Task<OrderEntity> CreateOrderAsync(CreatePaymentOrderInput createPaymentOrderInput);

    /// <summary>
    /// Метод получает скидку на услугу по ее типу и кол-ву месяцев.
    /// </summary>
    /// <param name="paymentMonth">Кол-во месяцев.</param>
    /// <param name="discountTypeEnum">Тип скидки на услугу</param>
    /// <returns>Скидка на услугу.</returns>
    Task<decimal> GetPercentDiscountAsync(short paymentMonth, DiscountTypeEnum discountTypeEnum);

    /// <summary>
    /// Метод обновляет статус заказа.
    /// </summary>
    /// <param name="paymentStatusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentStatusName">Русское название статуса заказа.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="orderId">Id заказа в БД.</param>
    Task<bool> UpdateOrderStatusAsync(string paymentStatusSysName, string paymentStatusName, string paymentId,
        long orderId);

    /// <summary>
    /// Метод создает возврат в БД.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="dateCreated">Дата создания возврата в ПС.</param>
    /// <param name="status">Статус возврата в ПС.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <returns>Данные возврата.</returns>
    Task<RefundEntity> CreateRefundAsync(string paymentId, decimal price, DateTime dateCreated, string status,
        string refundOrderId);
}