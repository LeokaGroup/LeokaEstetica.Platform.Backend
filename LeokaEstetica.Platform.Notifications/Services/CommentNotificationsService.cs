﻿using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using Microsoft.AspNetCore.SignalR;
using LeokaEstetica.Platform.Notifications.Models.Output;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений комментариев.
/// </summary>
public class CommentNotificationsService : ICommentNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly INotificationsRedisService _notificationsRedisService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Хаб.</param>
    /// <param name="notificationsRedisService">Сервис уведомлений кэша.</param>
    public CommentNotificationsService(IHubContext<NotifyHub> hubContext,
        INotificationsRedisService notificationsRedisService)
    {
        _hubContext = hubContext;
        _notificationsRedisService = notificationsRedisService;

    }
    /// <summary>
    /// Метод отправляет уведомление о том что комментарий к проекту не может быть пустым.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationCommentProjectIsNotEmptyAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningEmptyCommentProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}
