using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;

namespace LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;

/// <summary>
/// Класс реализует стратегию платежной системы ЮKassa.
/// </summary>
internal class YandexKassaStrategy : BasePaymentSystemStrategy
{
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public override async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
    {
        return new CreateOrderYandexKassaOutput();
    }
}