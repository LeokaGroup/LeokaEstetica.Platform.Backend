namespace LeokaEstetica.Platform.Models.Dto.Output.Orders;

/// <summary>
/// Класс результата выходной модели истории транзакций по заказам пользователя.
/// </summary>
public class HistoryResult
{
    /// <summary>
    /// Список транзакций.
    /// </summary>
    public IEnumerable<HistoryOutput> Histories { get; set; }

    /// <summary>
    /// Всего.
    /// </summary>
    public long Total => Histories.Count();
}