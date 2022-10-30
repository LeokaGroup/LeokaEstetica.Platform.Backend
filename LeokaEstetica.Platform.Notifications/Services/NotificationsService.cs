using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
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
        ILogService logger, 
        IRedisService redisService)
    {
        _hubContext = hubContext;
        _logger = logger;
        _redisService = redisService;
    }

    /// <summary>
    /// TODO: Будет изменяттся еще возможно. После тестирования уведомлений через All, иначе возможно придется слать через Clients.Client с userCode.
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotifySuccessSaveAsync(string title, string notifyText, string userCode)
    {
        // // Получаем ConnectionId из кэша.
        // var connectionId = await _redisService.GetConnectionIdCacheAsync(userCode);
        
        await _hubContext.Clients.All.SendAsync("SendNotifySuccessSave", new NotificationOutput
        {
            Title = title,
            Message = notifyText
        });
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
    /// TODO: После тестирования All будет понятно, убирать этот метод или нет.
    /// Метод валидирует входные параметры подключения SignalR.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    // private void ValidateConnection(string connectionId, string userCode)
    // {
    //     if (string.IsNullOrEmpty(connectionId))
    //     {
    //         throw new ArgumentNullException($"ConnectionId был пустым!");
    //     }
    //     
    //     if (string.IsNullOrEmpty(userCode))
    //     {
    //         throw new ArgumentNullException($"Код пользователя был пустым!");
    //     }
    // }
    
    /// <summary>
    /// Метод отправляет уведомление с предупреждением о пустом списке навыков пользователя. Пользователь значит не выбрал навыки.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationWarningSaveUserSkillsAsync(string title, string notifyText, string userCode)
    {
        await _hubContext.Clients.All.SendAsync("SendNotifySuccessSave", new NotificationOutput
        {
            Title = title,
            Message = notifyText
        });
    }
}