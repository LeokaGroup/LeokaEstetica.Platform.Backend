using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений доступа пользователя.
/// </summary>
internal sealed class AccessUserNotificationsService : IAccessUserNotificationsService
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Хаб.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public AccessUserNotificationsService(IHubContext<ChatHub> hubContext, 
        IConnectionService connectionService)
    {
        _hubContext = hubContext;
        _connectionService = connectionService;
    }

    /// <summary>
    /// Метод отправляет уведомление о предупреждении не заполненной анкеты пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningEmptyUserProfileAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningEmptyUserProfile", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление о блокировке профиля пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningBlockedUserProfileAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningBlockedUserProfile", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление о успешной отправке ссылки на восстановление пароля.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessLinkRestoreUserPasswordAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessLinkRestoreUserPassword", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}