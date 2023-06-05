namespace LeokaEstetica.Platform.Models.Dto.Output.Metrics;

/// <summary>
/// Класс выходной модели метрик новых пользователей.
/// </summary>
public class NewUserMetricsOutput
{
    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Текст приветствия нового пользователя.
    /// </summary>
    public string DisplayText { get; set; }

    /// <summary>
    /// Логин.
    /// </summary>
    public string Login { get; set; }
}