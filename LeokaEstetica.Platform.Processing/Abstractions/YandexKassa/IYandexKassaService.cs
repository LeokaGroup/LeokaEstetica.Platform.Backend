using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Enums;

namespace LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;

/// <summary>
/// Абстракция сервиса платежной системы ЮKassa.
/// </summary>
public interface IYandexKassaService
{
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные платежа.</returns>
    Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account);
    
    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId);
    
    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="amount">Данные о цене.</param>
    Task ConfirmPaymentAsync(string paymentId, Amount amount);
}