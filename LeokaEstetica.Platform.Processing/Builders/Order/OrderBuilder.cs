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
        await builder.FillMonthAsync();
        await builder.FillFareRuleNameAsync();
        await builder.CalculateFareRulePriceAsync();
    }
}