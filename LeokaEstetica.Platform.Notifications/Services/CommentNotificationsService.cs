using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using Microsoft.AspNetCore.SignalR;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений комментариев.
/// </summary>
internal sealed class CommentNotificationsService : ICommentNotificationsService
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Хаб.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public CommentNotificationsService(IHubContext<ChatHub> hubContext,
        IConnectionService connectionService)
    {
        _hubContext = hubContext;
        _connectionService = connectionService;

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
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningEmptyCommentProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление успешной записи комментария проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessCreatedCommentProjectAsync(string title, string notifyText, string notificationLevel,
        string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessCreatedCommentProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}
