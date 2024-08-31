using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;
using LeokaEstetica.Platform.Processing.Builders.Order;
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

    /// <inheritdoc />
    protected internal override async Task<ICreateOrderOutput> CreateOrderAsync(BaseOrderBuilder orderBuilder)
    {
        var result = await _yandexKassaService.CreateOrderAsync(orderBuilder);

        return result;
    }

    /// <inheritdoc />
    protected internal override async Task<PaymentStatusEnum> CheckOrderStatusAsync(BaseOrderBuilder orderBuilder)
    {
        var result = await _yandexKassaService.CheckOrderStatusAsync(orderBuilder);

        return result;
    }

    /// <inheritdoc />
    protected internal override async Task ConfirmPaymentAsync(BaseOrderBuilder orderBuilder)
    {
        await _yandexKassaService.ConfirmPaymentAsync(orderBuilder);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}