using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Builders.Order;
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
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Данные платежа.</returns>
    Task<ICreateOrderOutput> CreateOrderAsync(BaseOrderBuilder orderBuilder);
    
    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Статус платежа.</returns>
    Task<PaymentStatusEnum> CheckOrderStatusAsync(BaseOrderBuilder orderBuilder);
    
    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    Task ConfirmPaymentAsync(BaseOrderBuilder orderBuilder);
}