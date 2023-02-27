using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений.
/// </summary>
public sealed class NotificationsService : INotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;

    public NotificationsService(IHubContext<NotifyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// TODO: Будет изменяттся еще возможно. После тестирования уведомлений через All, иначе возможно придется слать через Clients.Client с userCode.
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotifySuccessSaveAsync(string title, string notifyText, string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotifySuccessSave", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }
}