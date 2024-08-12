namespace LeokaEstetica.Platform.Notifications.Models.Output;

/// <summary>
/// Класс выходной модели сообщения.
/// </summary>
public class NotificationOutput
{
    /// <summary>
    /// Заголовок уведомления.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Уровень уведомления.
    /// </summary>
    public string? NotificationLevel { get; set; }
}