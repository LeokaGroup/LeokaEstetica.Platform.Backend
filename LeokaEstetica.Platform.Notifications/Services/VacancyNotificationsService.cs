using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений вакансий.
/// </summary>
public class VacancyNotificationsService : IVacancyNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly INotificationsRedisService _notificationsRedisService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="notificationsRedisService">Сервис уведомлений кэша.</param>
    public VacancyNotificationsService(IHubContext<NotifyHub> hubContext, 
        INotificationsRedisService notificationsRedisService)
    {
        _hubContext = hubContext;
        _notificationsRedisService = notificationsRedisService;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном создании вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SendNotificationSuccessCreatedUserVacancyAsync(string title, string notifyText,
        string notificationLevel, long userId)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(userId.ToString());

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessCreatedUserVacancy", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при создании вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SendNotificationErrorCreatedUserVacancyAsync(string title, string notifyText,
        string notificationLevel, long userId)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(userId.ToString());

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorCreatedUserVacancy", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об предупреждении лимите вакансий по тарифу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SendNotificationWarningLimitFareRuleVacanciesAsync(string title, string notifyText,
        string notificationLevel, long userId)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(userId.ToString());

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningLimitFareRuleVacancies",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SendNotificationErrorDeleteVacancyAsync(string title, string notifyText, 
        string notificationLevel, long userId)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(userId.ToString());

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorDeleteVacancy",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SendNotificationSuccessDeleteVacancyAsync(string title, string notifyText,
        string notificationLevel, long userId)
    {
        var connectionId = await _notificationsRedisService.GetConnectionIdCacheAsync(userId.ToString());

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessDeleteVacancy",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
}