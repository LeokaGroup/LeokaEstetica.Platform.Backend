using LeokaEstetica.Platform.Base.Models.Dto;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.Vacancy;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;

/// <summary>
/// Класс события заказа.
/// </summary>
public class OrderEvent : BaseEventMessage
{
    /// <summary>
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }
    
    /// <summary>
    /// Системное название статуса заказа.
    /// </summary>
    public string? StatusSysName { get; set; }

    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string? PaymentId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Кол-во месяцев подписки.
    /// </summary>
    public short? Month { get; set; }

    /// <summary>
    /// Цена.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    public CurrencyTypeEnum Currency { get; set; }

    /// <summary>
    /// Тип заказа.
    /// </summary>
    public OrderTypeEnum OrderType { get; set; }
    
    /// <summary>
    /// Входная модель заказа на платное размещение вакансии.
    /// </summary>
    public OrderVacancyCacheInput? VacancyOrderData { get; set; }
}