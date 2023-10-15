using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Processing.Abstractions.YandexKassa;

namespace LeokaEstetica.Platform.Processing.Services.YandexKassa;

/// <summary>
/// Класс реализует методы сервиса платежной системы ЮKassa.
/// </summary>
internal sealed class YandexKassaService : IYandexKassaService
{
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public Task<CreateYandexKassaOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
    {
        throw new NotImplementedException();
    }
}