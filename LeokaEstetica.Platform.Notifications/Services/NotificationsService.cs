using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений.
/// </summary>
public sealed class NotificationsService : INotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly ILogService _logger;
    private readonly IDistributedCache _redis;

    public NotificationsService(IHubContext<NotifyHub> hubContext, 
        ILogService logger, 
        IDistributedCache redis)
    {
        _hubContext = hubContext;
        _logger = logger;
        _redis = redis;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="notifyText">Текст уведомления.</param>
    public async Task SendNotifySuccessSaveAsync(string notifyText)
    {
        await _hubContext.Clients
            .All
            .SendAsync("Receive", notifyText);
    }

    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SaveConnectionIdCacheAsync(string connectionId, string userCode)
    {
        try
        {
            ValidateConnection(connectionId, userCode);
            
            // Записываем ConnectionId в кэш редиса.
            await _redis.SetStringAsync(string.Concat(userCode + "_" + connectionId),
                ProtoBufExtensions.Serialize(connectionId),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод валидирует входные параметры подключения SignalR.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    private void ValidateConnection(string connectionId, string userCode)
    {
        if (string.IsNullOrEmpty(connectionId))
        {
            throw new ArgumentNullException($"ConnectionId был пустым!");
        }
        
        if (string.IsNullOrEmpty(userCode))
        {
            throw new ArgumentNullException($"Код пользователя был пустым!");
        }
    }
}