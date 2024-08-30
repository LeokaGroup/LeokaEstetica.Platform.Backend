using LeokaEstetica.Platform.Models.Dto.Input.Commerce.Vacancy;

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Класс билдера заказа платной публикации вакансии.
/// </summary>
internal class PostVacancyOrderBuilder : BaseOrderBuilder
{
    /// <summary>
    /// Входная модель заказа на платное размещение вакансии.
    /// </summary>
    public OrderVacancyCacheInput? VacancyOrderData { get; set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="orderCacheInput">Модель заказа для заполнения ее полей.</param>
    public PostVacancyOrderBuilder()
    {
    }
    
    public override Task FillMonthAsync()
    {
        throw new NotImplementedException("Не предполагается для билдера вакансий.");
    }

    public override Task FillFareRuleNameAsync()
    {
        throw new NotImplementedException();
    }

    public override Task CalculateFareRulePriceAsync()
    {
        throw new NotImplementedException();
    }
}