using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Processing.Enums;

namespace LeokaEstetica.Platform.Processing.Abstractions.PayMaster;

/// <summary>
/// Абстракция сервиса работы с платежной системой PayMaster.
/// </summary>
public interface IPayMasterService
{
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token);
    
    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId);
    
    /// <summary>
    /// Метод создает возврат в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="currency">Валюта.</param>
    /// <returns>Выходная модель.</returns>
    Task<CreateRefundOutput> CreateRefundAsync(string paymentId, decimal price, string currency);
    
    /// <summary>
    /// Метод проверяет статус возврата в ПС.
    /// </summary>
    /// <param name="refundId">Id возврата.</param>
    /// <param name="httpClient">HttpClient.</param>
    /// <returns>Статус возврата.</returns>
    Task<RefundStatusEnum> CheckRefundStatusAsync(string refundId, HttpClient httpClient);

    /// <summary>
    /// Метод создает чек возврата в ПС и отправляет его пользователю на почту.
    /// <param name="createReceiptInput">Входная модель.</param>
    /// </summary>
    /// <returns>Выходная модель чека.</returns>
    Task<CreateReceiptOutput> CreateReceiptRefundAsync(CreateReceiptPayMasterInput createReceiptInput);
}