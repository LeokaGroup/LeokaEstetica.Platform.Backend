using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Builders.Order;
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
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Данные платежа.</returns>
    protected internal abstract Task<ICreateOrderOutput> CreateOrderAsync(BaseOrderBuilder orderBuilder);
    
    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Статус платежа.</returns>
    protected internal abstract Task<PaymentStatusEnum> CheckOrderStatusAsync(BaseOrderBuilder orderBuilder);
    
    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    protected internal abstract Task ConfirmPaymentAsync(BaseOrderBuilder orderBuilder);
}