using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
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
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    internal async Task<ICreateOrderOutput> CreateOrderAsync(BasePaymentSystemStrategy strategy, Guid publicId,
        string account, string token)
    {
        if (strategy is not null)
        {
            return await strategy.CreateOrderAsync(publicId, account, token);
        }

        return null;
    }

    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    internal async Task<PaymentStatusEnum> CheckOrderStatusAsync(BasePaymentSystemStrategy strategy, string paymentId)
    {
        if (strategy is not null)
        {
            return await strategy.CheckOrderStatusAsync(paymentId);
        }

        return PaymentStatusEnum.None;
    }

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="amount">Данные о цене.</param>
    internal async Task ConfirmPaymentAsync(BasePaymentSystemStrategy strategy, string paymentId, Amount amount)
    {
        if (strategy is not null)
        {
            await strategy.ConfirmPaymentAsync(paymentId, amount);
        }
    }
}