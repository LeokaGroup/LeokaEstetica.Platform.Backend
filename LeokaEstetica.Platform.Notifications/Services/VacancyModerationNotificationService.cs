using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений модерации вакансий.
/// </summary>
internal sealed class VacancyModerationNotificationService : IVacancyModerationNotificationService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly IConnectionService _connectionService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public VacancyModerationNotificationService(IHubContext<NotifyHub> hubContext, 
        IConnectionService connectionService)
    {
        _hubContext = hubContext;
        _connectionService = connectionService;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении замечаний вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessCreateVacancyRemarksAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessCreateVacancyRemarks",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление о предупреждении при отправке замечаний вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningSendVacancyRemarksAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningSendVacancyRemarks",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успешной отправке замечаний вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessSendVacancyRemarksAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessSendVacancyRemarks",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
}