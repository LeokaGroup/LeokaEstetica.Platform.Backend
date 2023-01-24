using LeokaEstetica.Platform.Base.Models.Input.Processing;

namespace LeokaEstetica.Platform.Processing.Factories;

/// <summary>
/// Класс факторки создания заказа.
/// </summary>
public static class CreatePaymentOrderFactory
{
    /// <summary>
    /// Метод создает и заполняет объект заказа.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <param name="description">Описание тарифа.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="price">Стоимость тарифа.</param>
    /// <param name="paymentMonth">Кол-во мес.</param>
    /// <param name="currency">Валюта платежа.</param>
    /// <param name="created">Дата создания платежа.</param>
    /// <param name="orderStatusSysName">Системное название платежа.</param>
    /// <param name="orderStatusName">Название платежа.</param>
    /// <returns>Результирующий объект.</returns>
    public static CreatePaymentOrderInput Create(string paymentId, string fareRuleName, string description, long userId,
        decimal price, short paymentMonth, string currency, DateTime created, string orderStatusSysName,
        string orderStatusName)
    {
        var result = new CreatePaymentOrderInput
        {
            PaymentId = paymentId,
            Name = fareRuleName,
            Description = description,
            UserId = userId,
            Price = price,
            PaymentMonth = paymentMonth,
            Currency = currency,
            Created = created,
            PaymentStatusSysName = orderStatusSysName,
            PaymentStatusName = orderStatusName
        };

        return result;
    }
}