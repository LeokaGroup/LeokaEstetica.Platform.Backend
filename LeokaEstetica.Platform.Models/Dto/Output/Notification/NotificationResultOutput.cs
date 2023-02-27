namespace LeokaEstetica.Platform.Models.Dto.Output.Notification;

/// <summary>
/// Класс выходной модели результата уведомлений.
/// </summary>
public class NotificationResultOutput
{
    /// <summary>
    /// Список уведомлений.
    /// </summary>
    public IEnumerable<NotificationOutput> Notifications { get; set; }

    /// <summary>
    /// Кол-во уведомлений.
    /// </summary>
    public int Total => Notifications?.Count() ?? 0;
}