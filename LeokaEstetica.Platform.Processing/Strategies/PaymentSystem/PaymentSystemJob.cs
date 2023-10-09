using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

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
    internal async Task<CreateOrderOutput> CreateOrderAsync(BasePaymentSystemStrategy strategy, Guid publicId,
        string account, string token)
    {
        if (strategy is not null)
        {
            return await strategy.CreateOrderAsync(publicId, account, token);
        }

        return null;
    }
}