using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Structs;
using LeokaEstetica.Platform.Models.Entities.Commerce;

namespace LeokaEstetica.Platform.Database.Abstractions.Commerce;

/// <summary>
/// Абстракция репозитория коммерции.
/// </summary>
public interface ICommerceRepository
{
    /// <summary>
    /// Метод создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderInput">Входная модель.</param>
    /// <returns>Данные заказа.</returns>
    Task<OrderEntity> CreateOrderAsync(CreatePaymentOrderInput createPaymentOrderInput);

    /// <summary>
    /// Метод получает скидку на услугу по ее типу и кол-ву месяцев.
    /// </summary>
    /// <param name="paymentMonth">Кол-во месяцев.</param>
    /// <param name="discountTypeEnum">Тип скидки на услугу</param>
    /// <returns>Скидка на услугу.</returns>
    Task<DiscountStruct> GetPercentDiscountAsync(short paymentMonth, DiscountTypeEnum discountTypeEnum);
}