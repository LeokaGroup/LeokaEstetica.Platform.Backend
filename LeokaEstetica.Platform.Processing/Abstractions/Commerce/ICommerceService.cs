using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;

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

    /// <summary>
    /// Метод вычисляет, есть ли остаток с прошлой подписки пользователя для учета ее как скидку при оформлении новой подписки.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <returns>Сумма остатка, если она есть.</returns>
    Task<OrderFreeOutput> CheckFreePriceAsync(string account, Guid publicId, short month);

    /// <summary>
    /// Метод проверяет заполнение анкеты пользователя.
    /// Если не заполнена, то нельзя оформить заказ.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> IsProfileEmptyAsync(string account, string token);
}