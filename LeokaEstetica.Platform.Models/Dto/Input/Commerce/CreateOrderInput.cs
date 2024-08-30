using LeokaEstetica.Platform.Models.Dto.Input.Commerce.Vacancy;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce;

/// <summary>
/// Класс входной модели создания заказа в кэше.
/// </summary>
public class CreateOrderInput
{
    /// <summary>
    /// Тип заказа.
    /// </summary>
    public OrderTypeEnum OrderType { get; set; }

    /// <summary>
    /// Входная модель заказа тарифа.
    /// </summary>
    public FareRuleCacheInput? FareRuleCache { get; set; }

    /// <summary>
    /// Входная модель заказа на платное размещение вакансии.
    /// </summary>
    public OrderVacancyCacheInput? VacancyOrderData { get; set; }

    /// <summary>
    /// Публичный ключ.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Кол-во мес. подписки.
    /// </summary>
    public short? PaymentMonth { get; set; }
}