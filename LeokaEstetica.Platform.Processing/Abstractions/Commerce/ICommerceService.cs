using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;

namespace LeokaEstetica.Platform.Processing.Abstractions.Commerce;

/// <summary>
/// TODO: Отрефачить разбив логику заказов в отдельный сервис OrderService.
/// Абстракция сервиса коммерции.
/// </summary>
public interface ICommerceService
{
    /// <summary>
    /// Метод создает заказ в кэше.
    /// </summary>
    /// <param name="createOrderCache">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные заказа добавленного в кэш.</returns>
    Task<CreateOrderCache> CreateOrderCacheAsync(CreateOrderCacheInput createOrderCacheInput, string account);

    /// <summary>
    /// Метод получает услуги и сервисы заказа из кэша.
    /// </summary>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Услуги и сервисы заказа.</returns>
    Task<CreateOrderCache> GetOrderProductsCacheAsync(Guid publicId, string account);

    /// <summary>
    /// Метод вычисляет сумму с оставшихся дней подписки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Сумма.</returns>
    Task<decimal> CalculatePriceSubscriptionFreeDaysAsync(long userId, long orderId);
}