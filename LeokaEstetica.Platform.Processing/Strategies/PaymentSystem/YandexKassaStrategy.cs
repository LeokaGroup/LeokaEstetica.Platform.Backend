using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Enums;

namespace LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;

/// <summary>
/// Класс реализует стратегию платежной системы ЮKassa.
/// </summary>
internal class YandexKassaStrategy : BasePaymentSystemStrategy
{
    private readonly IYandexKassaService _yandexKassaService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="yandexKassaService">Сервис ЮKassa.</param>
    public YandexKassaStrategy(IYandexKassaService yandexKassaService)
    {
        _yandexKassaService = yandexKassaService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные платежа.</returns>
    public override async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account)
    {
        var result = await _yandexKassaService.CreateOrderAsync(publicId, account);

        return result;
    }

    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <returns>Статус платежа.</returns>
    public override async Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId)
    {
        var result = await _yandexKassaService.CheckOrderStatusAsync(paymentId);

        return result;
    }

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="amount">Данные о цене.</param>
    public override async Task ConfirmPaymentAsync(string paymentId, Amount amount)
    {
        await _yandexKassaService.ConfirmPaymentAsync(paymentId, amount);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}