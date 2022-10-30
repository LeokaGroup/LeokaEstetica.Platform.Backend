using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Redis.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений.
/// </summary>
public sealed class NotificationsService : INotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly ILogService _logger;
    private readonly IRedisService _redisService;

    public NotificationsService(IHubContext<NotifyHub> hubContext, 
        ILogService logger, IRedisService redisService)
    {
        _hubContext = hubContext;
        _logger = logger;
        _redisService = redisService;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotifySuccessSaveAsync(string notifyText, string userCode)
    {
        // // Получаем ConnectionId из кэша.
        // var connectionId = await _redisService.GetConnectionIdCacheAsync(userCode);
        
        await _hubContext.Clients.All.SendAsync("SendNotifySuccessSave", notifyText);
    }

    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    // public async Task SaveConnectionIdCacheAsync(string connectionId, string userCode)
    // {
    //     try
    //     {
    //         ValidateConnection(connectionId, userCode);
    //         await _redisService.SaveConnectionIdCacheAsync(connectionId, userCode);
    //     }
    //     
    //     catch (Exception ex)
    //     {
    //         await _logger.LogErrorAsync(ex);
    //         throw;
    //     }
    // }

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