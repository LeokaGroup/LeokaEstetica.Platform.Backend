using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

namespace LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;

/// <summary>
/// Класс события заказа на платную публикацию вакансии.
/// </summary>
public class PostVacancyOrderEvent : OrderEvent
{
    /// <summary>
    /// Входная модель заказа на платное размещение вакансии.
    /// </summary>
    public VacancyInput? VacancyOrderData { get; set; }
}