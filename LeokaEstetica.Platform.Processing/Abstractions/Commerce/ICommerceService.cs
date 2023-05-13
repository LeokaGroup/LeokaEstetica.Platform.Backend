using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;

namespace LeokaEstetica.Platform.Processing.Abstractions.Commerce;

/// <summary>
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
}