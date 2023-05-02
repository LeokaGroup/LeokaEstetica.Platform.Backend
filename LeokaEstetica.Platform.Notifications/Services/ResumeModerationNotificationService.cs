using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений модерации анкет.
/// </summary>
public class ResumeModerationNotificationService : IResumeModerationNotificationService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly INotificationsRedisService _notificationsRedisService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="notificationsRedisService">Сервис уведомлений кэша.</param>
    public ResumeModerationNotificationService(IHubContext<NotifyHub> hubContext, 
        INotificationsRedisService notificationsRedisService)
    {
        _hubContext = hubContext;
        _notificationsRedisService = notificationsRedisService;
    }
    
    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении замечаний анкет.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessCreateResumeRemarksAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessCreateResumeRemarks",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление о предупреждении при отправке замечаний анкет.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningSendResumeRemarksAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningSendResumeRemarks",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успешной отправке замечаний анкет.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessSendResumeRemarksAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessSendResumeRemarks",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
}