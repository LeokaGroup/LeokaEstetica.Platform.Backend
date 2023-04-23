using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс уведомлений модераторов.
/// </summary>
public class VacancyModerationNotificationsService : IVacancyModerationNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly INotificationsRedisService _notificationsRedisService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="notificationsRedisService">Сервси уведомлений кеша.</param>
    public VacancyModerationNotificationsService(IHubContext<NotifyHub> hubContext, 
        INotificationsRedisService notificationsRedisService)
    {
        _hubContext = hubContext;
        _notificationsRedisService = notificationsRedisService;
    }

    /// <summary>
    /// Отправляет уведомление модератору.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="message">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен модератора.</param>
    public async Task SendNotificationWarningApproveVacancyAsync(string title, string message, string notificationLevel, 
        string token)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorApproveProject", new NotificationOutput
            {
                Title = title,
                Message = message,
                NotificationLevel = notificationLevel
            });
    }
}