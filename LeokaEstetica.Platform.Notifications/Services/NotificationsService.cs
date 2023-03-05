using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений.
/// </summary>
public sealed class NotificationsService : INotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly INotificationsRedisService _notificationsRedisService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="notificationsRedisService">Сервис уведомлений кэша.</param>
    public NotificationsService(IHubContext<NotifyHub> hubContext, 
        INotificationsRedisService notificationsRedisService)
    {
        _hubContext = hubContext;
        _notificationsRedisService = notificationsRedisService;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SendNotifySuccessSaveAsync(string title, string notifyText, string notificationLevel, 
        long userId)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(userId.ToString());

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotifySuccessSave", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}