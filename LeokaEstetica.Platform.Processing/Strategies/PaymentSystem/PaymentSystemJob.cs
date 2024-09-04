using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Builders.Order;
using LeokaEstetica.Platform.Processing.Enums;

namespace LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;

/// <summary>
/// Класс представляет семейство джоб для работы с определенной платежной системой.
/// </summary>
internal class PaymentSystemJob
{
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="strategy">Стратегия, по которой будем оформлять заказ.</param>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Данные платежа.</returns>
    protected internal async Task<ICreateOrderOutput> CreateOrderAsync(BasePaymentSystemStrategy strategy,
        BaseOrderBuilder orderBuilder)
    {
        if (strategy is not null)
        {
            return await strategy.CreateOrderAsync(orderBuilder);
        }

        return null;
    }

    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="strategy">Стратегия, по которой будем оформлять заказ.</param>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Статус платежа.</returns>
    protected internal async Task<PaymentStatusEnum> CheckOrderStatusAsync(BasePaymentSystemStrategy strategy,
        BaseOrderBuilder orderBuilder)
    {
        if (strategy is not null)
        {
            return await strategy.CheckOrderStatusAsync(orderBuilder);
        }

        return PaymentStatusEnum.None;
    }

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="strategy">Стратегия, по которой будем оформлять заказ.</param>
    /// <param name="orderBuilder">Билдер заказа.</param>
    protected internal async Task ConfirmPaymentAsync(BasePaymentSystemStrategy strategy,
        BaseOrderBuilder orderBuilder)
    {
        if (strategy is not null)
        {
            await strategy.ConfirmPaymentAsync(orderBuilder);
        }
    }
}