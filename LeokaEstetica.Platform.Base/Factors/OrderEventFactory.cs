using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Класс наполнения данных события заказов.
/// </summary>
public static class OrderEventFactory
{
    /// <summary>
    /// TODO: Засунуть все параметры в модель. Их слишком много уже тут.
    /// Метод наполняет данными событие заказа.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="statusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <returns>Результирующая модель.</returns>
    public static OrderEvent CreateOrderEvent(long orderId, string statusSysName, string paymentId, long userId,
        Guid publicId, short? month, decimal price, CurrencyTypeEnum currency)
    {
        return new OrderEvent
        {
            OrderId = orderId,
            StatusSysName = statusSysName,
            PaymentId = paymentId,
            CreatedBy = userId,
            PublicId = publicId,
            Month = month,
            Price = price,
            Currency = currency
        };
    }

    /// <summary>
    /// TODO: Засунуть все параметры в модель. Их слишком много уже тут.
    /// Метод наполняет данными событие заказа.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="statusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <returns>Результирующая модель.</returns>
    public static PostVacancyOrderEvent CreatePostVacancyOrderEvent(long orderId, string statusSysName,
        string paymentId, long userId, Guid publicId, short? month, decimal price, CurrencyTypeEnum currency,
        VacancyInput vacancyOrderData)
    {
        return new PostVacancyOrderEvent
        {
            OrderId = orderId,
            StatusSysName = statusSysName,
            PaymentId = paymentId,
            CreatedBy = userId,
            PublicId = publicId,
            Month = month,
            Price = price,
            Currency = currency,
            VacancyOrderData = vacancyOrderData
        };
    }
}