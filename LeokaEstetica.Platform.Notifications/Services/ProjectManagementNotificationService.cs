using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений модуля УП - управление проектами.
/// </summary>
internal sealed class ProjectManagementNotificationService : IProjectManagementNotificationService
{
    private readonly IHubContext<ProjectManagementHub> _hubContext;
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public ProjectManagementNotificationService(IHubContext<ProjectManagementHub> hubContext,
        IConnectionService connectionService)
    {
        _hubContext = hubContext;
        _connectionService = connectionService;
    }

    /// <inheritdoc />
    public async Task SendNotifySuccessPlaningSprintAsync(string title, string notifyText, string notificationLevel,
        string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotifySuccessPlaningSprint", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <inheritdoc />
    public async Task SendNotifySuccessIncludeEpicTaskAsync(string title, string notifyText, string notificationLevel,
        string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotifySuccessIncludeEpicTask", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <inheritdoc />
    public async Task SendNotifyErrorIncludeEpicTaskAsync(string title, string notifyText, string notificationLevel,
        string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotifyErrorIncludeEpicTask", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <inheritdoc />
    public async Task SendNotifyWarningDublicateProjectTaskAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotifyWarningDublicateProjectTask", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <inheritdoc />
    public async Task SendClassificationNetworkMessageResultAsync(string message, string connectionId, long dialogId)
    {
        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendClassificationNetworkMessageResult", new ClassificationNetworkMessageOutput
            {
                Response = message,
                ConnectionId = connectionId,
                IsMyMessage = true,
                ScrumMasterAiEventType = ScrumMasterAiEventTypeEnum.Message.ToString(),
                DialogId = dialogId
            });
    }

    /// <inheritdoc />
    public async Task SendNotifySuccessUpdateRolesAsync(string title, string notifyText, string notificationLevel,
        string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotifySuccessUpdateRoles", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }
}