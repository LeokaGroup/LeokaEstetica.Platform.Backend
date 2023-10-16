using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;

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
}