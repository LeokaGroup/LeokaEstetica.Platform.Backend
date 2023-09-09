using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений тикетов.
/// </summary>
internal sealed class TicketNotificationService : ITicketNotificationService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public TicketNotificationService(IHubContext<NotifyHub> hubContext, 
        IConnectionService connectionService)
    {
        _hubContext = hubContext;
        _connectionService = connectionService;
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при закрытии тикета.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorCloseTicketAsync(string title, string notifyText, string notificationLevel,
        string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorCloseTicket", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}