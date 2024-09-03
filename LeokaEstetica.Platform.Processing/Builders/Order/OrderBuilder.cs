using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Processing.BuilderData;

namespace LeokaEstetica.Platform.Processing.Builders.Order;

/// <summary>
/// Класс билдера заказа.
/// </summary>
internal class OrderBuilder
{
    /// <summary>
    /// Метод строит нужный тип объекта.
    /// </summary>
    /// <param name="builder">Строитель, который занимается построением нужного объекта.</param>
    /// <returns>Результирующая модель.</returns>
    public async Task BuildAsync(BaseOrderBuilder builder)
    {
        builder.OrderData ??= new OrderData();
        
        // В кейсе с платной публикацией вакансий не важен срок подписки.
        if (builder.OrderData.OrderType != OrderTypeEnum.CreateVacancy)
        {
            await builder.FillMonthAsync();
        }
        
        await builder.FillFareRuleNameAsync();
        await builder.CalculateFareRulePriceAsync();
        await builder.FillOrderTypeAsync();
    }
}