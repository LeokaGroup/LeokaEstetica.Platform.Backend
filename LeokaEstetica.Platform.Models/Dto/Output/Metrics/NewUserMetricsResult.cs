namespace LeokaEstetica.Platform.Models.Dto.Output.Metrics;

/// <summary>
/// Класс результата метрик новых пользователей.
/// </summary>
public class NewUserMetricsResult
{
    /// <summary>
    /// Список новых пользователей.
    /// </summary>
    public IEnumerable<NewUserMetricsOutput> NewUsers { get; set; }

    /// <summary>
    /// Всего.
    /// </summary>
    public int Total => NewUsers.Count();
}