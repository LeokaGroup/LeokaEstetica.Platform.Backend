using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений вакансий.
/// </summary>
public class VacancyNotificationsService : IVacancyNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    public VacancyNotificationsService(IHubContext<NotifyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном создании вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotificationSuccessCreatedUserVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessCreatedUserVacancy", new NotificationOutput
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
    public async Task SendNotificationWarningLimitFareRuleVacanciesAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningLimitFareRuleVacancies",
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
    public async Task SendNotificationErrorDeleteVacancyAsync(string title, string notifyText, 
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorDeleteVacancy",
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
    public async Task SendNotificationSuccessDeleteVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessDeleteVacancy",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}