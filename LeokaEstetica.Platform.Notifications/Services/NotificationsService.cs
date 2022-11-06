using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений.
/// </summary>
public sealed class NotificationsService : INotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;

    public NotificationsService(IHubContext<NotifyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// TODO: Будет изменяттся еще возможно. После тестирования уведомлений через All, иначе возможно придется слать через Clients.Client с userCode.
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotifySuccessSaveAsync(string title, string notifyText, string notificationLevel, string userCode)
    {
        // // Получаем ConnectionId из кэша.
        // var connectionId = await _redisService.GetConnectionIdCacheAsync(userCode);
        
        await _hubContext.Clients.All.SendAsync("SendNotifySuccessSave", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
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
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationWarningSaveUserSkillsAsync(string title, string notifyText, string notificationLevel, string userCode)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningSaveUserSkills", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление с предупреждением о пустом списке целей пользователя. Пользователь значит не выбрал навыки.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationWarningSaveUserIntentsAsync(string title, string notifyText, string notificationLevel,
        string userCode)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningSaveUserIntents", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText, string notificationLevel, string userCode)
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText, string notificationLevel,
        string userCode)
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText, string notificationLevel,
        string userCode)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningDublicateUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }
}