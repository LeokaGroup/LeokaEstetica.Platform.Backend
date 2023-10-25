using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Abstractions.Commerce;

/// <summary>
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
    /// Метод проставляет статус заказа подтвержден.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="paymentStatusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentStatusName">Русское название статуса заказа.</param>
    Task SetStatusConfirmByPaymentIdAsync(string paymentId, string paymentStatusSysName, string paymentStatusName);

    /// <summary>
    /// Метод создает возврат в БД.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="dateCreated">Дата создания возврата в ПС.</param>
    /// <param name="status">Статус возврата в ПС.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <param name="isManual">Признак ручного создания возврата.</param>
    /// <returns>Данные возврата.</returns>
    Task<RefundEntity> CreateRefundAsync(string paymentId, decimal price, DateTime dateCreated, string status,
        string refundOrderId, bool isManual);
    
    /// <summary>
    /// Метод обновляет статус возврата.
    /// </summary>
    /// <param name="refundStatusName">Русское название статуса возврата.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="refundId">Id возврата в БД.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    Task<bool> UpdateRefundStatusAsync(string refundStatusName, string paymentId, long refundId, string refundOrderId);
    
    /// <summary>
    /// Метод создает чек возврата в БД.
    /// </summary>
    /// <param name="createReceiptOutput">Модель результата из ПС.</param>
    /// <returns>Данные чека.</returns>
    Task<ReceiptEntity> CreateReceiptRefundAsync(CreateReceiptOutput createReceiptOutput);

    /// <summary>
    /// Метод проверяет, существует ли уже такой возврат.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> IfExistsRefundAsync(string orderId);
    
    /// <summary>
    /// Метод получает список возвратов для КЦ, которые не обработаны.
    /// </summary>
    /// <returns>Список необработанных возвратов.</returns>
    Task<IEnumerable<RefundEntity>> GetUnprocessedRefundsAsync();
}