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
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public override async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
    {
        var result = await _yandexKassaService.CreateOrderAsync(publicId, account, token);

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

    #endregion

    #region Приватные методы.

    

    #endregion
}