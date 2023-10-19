using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Enums;

namespace LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;

/// <summary>
/// Базовый класс платежных систем.
/// </summary>
internal abstract class BasePaymentSystemStrategy
{
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public abstract Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token);
    
    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    public abstract Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId);
    
    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="amount">Данные о цене.</param>
    public abstract Task ConfirmPaymentAsync(string paymentId, Amount amount);
}