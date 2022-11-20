using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений проектов.
/// </summary>
public sealed class ProjectNotificationsService : IProjectNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;

    public ProjectNotificationsService(IHubContext<NotifyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText, string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessCreatedUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText, string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorCreatedUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление о дубликате проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText, string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningDublicateUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }
}