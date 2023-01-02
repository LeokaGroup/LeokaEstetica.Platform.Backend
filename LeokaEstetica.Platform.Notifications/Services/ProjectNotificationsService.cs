using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений проектов.
/// </summary>
public sealed class ProjectNotificationsService : IProjectNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;

    public ProjectNotificationsService(IHubContext<NotifyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
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
    public async Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
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
    public async Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningDublicateUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при изменении проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessUpdatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessUpdatedUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при изменении проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationErrorUpdatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorUpdatedUserProject", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление об успешной привязке вакансии к проекту.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessAttachProjectVacancy", new NotificationOutput
        {
            Title = title,
            Message = notifyText,
            NotificationLevel = notificationLevel
        });
    }

    /// <summary>
    /// Метод отправляет уведомление об дубликате при привязке вакансии к проекту.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationErrorDublicateAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorDublicateAttachProjectVacancy",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessProjectResponseAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessProjectResponse",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения об отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationWarningProjectResponseAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningProjectResponse",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения о не найденных пользователях по поисковому запросу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    public async Task SendNotificationWarningSearchProjectTeamMemberAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningSearchProjectTeamMember",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}