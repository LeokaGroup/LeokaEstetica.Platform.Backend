using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;

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
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token);
}