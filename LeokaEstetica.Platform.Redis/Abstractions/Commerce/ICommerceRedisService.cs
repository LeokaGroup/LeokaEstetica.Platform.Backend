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
    Task CreateOrderCacheAsync(string key, CreateOrderCache createOrderCache);

    /// <summary>
    /// Метод получает заказ из кэша.
    /// </summary>
    /// <param name="key">Ключ добавленного заказа.</param>
    /// <returns>Данные заказа из кэша.</returns>
    Task<CreateOrderCache> GetOrderCacheAsync(string key);
}