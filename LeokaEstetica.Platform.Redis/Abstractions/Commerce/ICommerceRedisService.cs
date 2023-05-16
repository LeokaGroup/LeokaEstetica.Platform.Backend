using LeokaEstetica.Platform.Models.Dto.Common.Cache;

namespace LeokaEstetica.Platform.Redis.Abstractions.Commerce;

/// <summary>
/// Абстракция кэша коммерции.
/// </summary>
public interface ICommerceRedisService
{
    /// <summary>
    /// Метод создает заказ в кэше.
    /// </summary>
    /// <param name="key">Ключ добавленного заказа.</param>
    /// <param name="createOrderCache">Модель заказа для хранения в кэше.</param>
    /// <returns>Данные заказа добавленного в кэш.</returns>
    Task<CreateOrderCache> CreateOrderCacheAsync(string key, CreateOrderCache createOrderCache);

    /// <summary>
    /// Метод получает заказ из кэша.
    /// </summary>
    /// <param name="key">Ключ добавленного заказа.</param>
    /// <returns>Данные заказа из кэша.</returns>
    Task<CreateOrderCache> GetOrderCacheAsync(string key);

    /// <summary>
    /// Метод создает ключ для работы с заказом, который в кэше.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <returns>Ключ для добавления заказа в кэш.</returns>
    Task<string> CreateOrderCacheKeyAsync(long userId, Guid publicId);
}